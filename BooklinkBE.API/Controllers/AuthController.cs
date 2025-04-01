using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BooklinkBE.API.Entities;
using BooklinkBE.API.Options;
using BooklinkBE.API.Services.Implementations;
using BooklinkBE.API.Utils;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BooklinkBE.API.Controllers;
[ApiController]
[Route("v1/api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtSettings _jwtSettings;
    private readonly EmailSenderService _emailSenderService;
    private readonly EnvironmentOptions _environmentOptions;

    public AuthController(
        AppDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        EmailSenderService emailSenderService,
        [FromServices]IOptions<EnvironmentOptions> environmentOptions,
        [FromServices]IOptions<JwtSettings> options
        )
    {
        _dbContext = dbContext;
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtSettings = options.Value;
        _emailSenderService = emailSenderService;
        _environmentOptions = environmentOptions.Value;
    }
    
    [HttpPost("register")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult> Register([FromBody] Register model)
{
    var newUser = new User
    {
        Id = Guid.NewGuid(),
        Email = model.Email,
        UserName = model.Email,
        NormalizedEmail = model.Email.ToLower(),
        NormalizedUserName = model.Email.ToLower(),
        SecurityStamp = Guid.NewGuid().ToString(),
        CreatedAt = DateTime.UtcNow,
        AccessFailedCount = 0
    };

    Console.WriteLine($"{newUser.Id} - {newUser.Email}, {newUser.UserName}, {newUser.CreatedAt}");

    // Create user with password
    var createUserResult = await _userManager.CreateAsync(newUser, model.Password);
    if (!createUserResult.Succeeded)
    {
        ModelState.AddModelError(string.Empty, string.Join("\n", createUserResult.Errors.Select(e => e.Description)));
        return ValidationProblem(ModelState);
    }
    
    newUser = await _userManager.FindByEmailAsync(model.Email);
    if (newUser == null)
    {
        return StatusCode(500, "User not found after creation.");
    }

    // Generate email confirmation token
    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
    if (string.IsNullOrWhiteSpace(token))
    {
        return StatusCode(500, "Failed to generate email confirmation token.");
    }

    var confirmationLink = $"{_environmentOptions.FrontendHostUrl}{_environmentOptions.FrontendConfirmUrl}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(newUser.Email.ToLower())}";
    Console.WriteLine(confirmationLink);

    await _emailSenderService.AddEmail("Registrace", confirmationLink, newUser.Email, newUser.UserName);

    return Ok(new { Message = "User registered successfully. Please confirm your email.", ConfirmationLink = confirmationLink });
}

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] Login model)
    {
        var normalizedEmail = model.Email.ToUpperInvariant();
        var user = await _userManager
            .Users
            .SingleOrDefaultAsync(x => x.EmailConfirmed && x.NormalizedEmail == normalizedEmail);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "LOGIN_FAILED");
            return ValidationProblem(ModelState);
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "LOGIN_FAILED");
            return ValidationProblem(ModelState);
        }

        var accessToken = GenerateAccessToken(user.Id, model.Email, user.UserName!, _jwtSettings.AccessTokenExpirationInMinutes);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id, _jwtSettings.RefreshTokenExpirationInDays);
        Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // For HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
        });
        return Ok(new { token = accessToken, email = user.Email, id = user.Id });
    }

    /// <summary>
    /// unescape token before sending
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("validate-token")]
    public async Task<ActionResult> ValidateToken([FromBody] Token model)
    {
        Console.WriteLine("Validating token...");
        var normalizedMail = model.Email.ToUpperInvariant();
        var user = await _userManager
            .Users
            .SingleOrDefaultAsync(x => !x.EmailConfirmed && x.NormalizedEmail == normalizedMail);

        if (user == null)
        {
            ModelState.AddModelError<Token>(x => x.SessionToken, "INVALID_TOKEN");
            return ValidationProblem(ModelState);
        }

        var check = await _userManager.ConfirmEmailAsync(user, model.SessionToken);
        if (!check.Succeeded)
        {
            ModelState.AddModelError<Token>(x => x.SessionToken, "INVALID_TOKEN");
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("user-info")]
    public async Task<ActionResult<LoggedUser>> GetUserInfo()
    {
        if (!User.Identities.Any(x => x.IsAuthenticated))
        {
            return new LoggedUser
            {
                Id = default,
                Name = null,
                IsAuthenticated = false,
                IsAdmin = false,
            };
        }

        var id = User.GetUserId();
        var user = await _userManager.Users
            .Where(x => x.Id == id)
            .AsNoTracking()
            .SingleAsync();

        var loggedModel = new LoggedUser
        {
            Id = user.Id,
            Name = user.Email,
            IsAuthenticated = true,
            IsAdmin = false,
        };

        return loggedModel;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("RefreshToken", out var incomingToken))
        {
            return Unauthorized(new { Message = "refresh token not found" });
        }

        var hashedToken = Hash(incomingToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hashedToken);

        if (storedToken == null || storedToken.ExpiresAt < DateTimeOffset.UtcNow || storedToken.RevokedAt != null)
        {
            return Unauthorized(new { Message = "Invalid or expired refresh token" });
        }

        // Generate new access and refresh tokens
        var user = await _dbContext.Users.FindAsync(storedToken.UserId);
        if (user == null)
        {
            return Unauthorized();
        }

        // Generate new tokens
        var newAccessToken = GenerateAccessToken(user.Id, user.Email!, user.UserName!, _jwtSettings.AccessTokenExpirationInMinutes);
        var newRefreshToken = await GenerateRefreshTokenAsync(user.Id, _jwtSettings.RefreshTokenExpirationInDays);

        storedToken.RevokedAt = DateTimeOffset.Now;
        await _dbContext.SaveChangesAsync();

        Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // For HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
        });
        return Ok(new
        {
            Token = newAccessToken,
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("RefreshToken", out var incomingToken))
        {
            return NoContent();
        }

        var hashedToken = Hash(incomingToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hashedToken);

        if (storedToken == null || storedToken.ExpiresAt < DateTimeOffset.Now || storedToken.RevokedAt != null)
        {
            return NoContent();
        }

        storedToken.ExpiresAt = DateTimeOffset.Now;
        await _dbContext.SaveChangesAsync();

        Response.Cookies.Delete("RefreshToken");
        return NoContent();
    }

    [Authorize]
    [HttpGet("testMeBeforeLoginAndAfter")]
    public ActionResult TestMeBeforeLoginAndAfter()
    {
        return Ok("Succesfully reached endpoint!");
    }

    private async Task<string> GenerateRefreshTokenAsync(Guid userId, int expirationInDays)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var data = Request.Headers.UserAgent.ToString();

        var now = DateTime.UtcNow;
        _dbContext.Add(new RefreshToken
            
        {
            UserId = userId,
            Token = Hash(refreshToken),
            CreatedAt = now,
            ExpiresAt = now.AddDays(expirationInDays),
            RequestInfo = data,
        });
        await _dbContext.SaveChangesAsync();
        return refreshToken;
    }

    private string GenerateAccessToken(Guid userId, string email, string username, int expirationInMinutes)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString().ToLowerInvariant()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Name, username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expirationInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string Hash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);

    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            return Ok(new { message = "If that email exists, a reset link has been sent." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetUrl = $"{_environmentOptions.FrontendHostUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

        await _emailSenderService.AddEmail("Password Reset", resetUrl, user.Email, user.UserName);

        return Ok(new { message = "Password reset link sent to email." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid request." });
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
        }

        return Ok(new { message = "Password has been reset successfully." });
    }
}