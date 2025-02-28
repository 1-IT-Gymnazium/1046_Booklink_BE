using System.Text;
using BooklinkBE.API.BackgroundWorkers;
using BooklinkBE.API.Options;
using BooklinkBE.API.Services.Implementations;
using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// database conntection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>(options =>
        options.SignIn.RequireConfirmedAccount = true
    )
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityCore<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT Settings not configured properly in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience
    };
});

// Checks if program runs production or development version
Console.WriteLine($"Environment: {builder.Configuration["ASPNETCORE_ENVIRONMENT"]}");

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<EnvironmentOptions>(builder.Configuration.GetSection("EnvironmentSettings"));

builder.Services.AddScoped<EmailSenderService>();

// Background Services
builder.Services.AddHostedService<EmailSenderBackgroundService>();

// Business Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookshelfService, BookshelfService>();
builder.Services.AddScoped<IRealEstateService, RealEstateService>();

// API Controllers
builder.Services.AddControllers();

// Swagger Configuration (API Documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Booklink API", Version = "v1" });

    // Configure JWT Authentication in Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token without the 'Bearer' prefix.\n\nExample: abc123xyz"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Frontend configuraton
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Booklink API v1");
    options.RoutePrefix = string.Empty; // This makes Swagger the default page
});


app.UseHttpsRedirection(); // Enable HTTPS redirection
app.UseCors("AllowAngularApp"); // Enable CORS for frontend

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();