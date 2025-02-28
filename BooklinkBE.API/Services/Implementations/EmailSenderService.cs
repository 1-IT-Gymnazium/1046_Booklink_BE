using System.Net.Mail;
using BooklinkBE.API.Options;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BooklinkBE.API.Services.Implementations;

public class EmailSenderService
{
    private readonly AppDbContext _dbContext;
    private readonly SmtpOptions _smtpOptions;
    private readonly EnvironmentOptions _envOptions;

    public EmailSenderService(AppDbContext appDbContext, IOptions<EnvironmentOptions> envOptions, IOptions<SmtpOptions> options)
    {
        _dbContext = appDbContext;
        _smtpOptions = options.Value;
        _envOptions = envOptions.Value;
    }

    public async Task AddEmail(string subject, string body, string recipientEmail, string? recipientName = null, string? fromEmail = null, string? fromName = null)
    {
        Console.WriteLine($"[DEBUG] Adding email - To: {recipientEmail}, Name: {recipientName}, From: {fromEmail ?? _envOptions.SenderEmail}, Subject: {subject}");
        var message = new EmailMessage
        {
            Id = Guid.NewGuid(),
            Subject = subject,
            Body = body,
            RecipientEmail = recipientEmail,
            RecipientName = recipientName,
            FromEmail = fromEmail ?? _envOptions.SenderEmail,
            FromName = fromName ?? _envOptions.SenderName,
            Sent = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Add(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SendEmailsAsync()
    {
        var unsentMails = await _dbContext.EmailMessages.Where(x => !x.Sent).ToListAsync();
        foreach (var unsent in unsentMails)
        {
            var mail = new MailMessage
            {
                Subject = unsent.Subject,
                Body = unsent.Body,
                IsBodyHtml = false,
                From = new MailAddress(unsent.FromEmail, unsent.FromName),
                
            };
            mail.To.Add(new MailAddress(unsent.RecipientEmail, unsent.RecipientName ?? "")); // kolekce prvků, nelze přidat při inicializaci 

            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
                await smtp.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
                await smtp.SendAsync((MimeMessage)mail);

                unsent.Sent = true;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}