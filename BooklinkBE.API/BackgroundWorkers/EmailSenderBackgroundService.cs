using BooklinkBE.API.Options;
using BooklinkBE.API.Services.Implementations;
using Microsoft.Extensions.Options;

namespace BooklinkBE.API.BackgroundWorkers;

public class EmailSenderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly SmtpOptions _smtpOptions;

    public EmailSenderBackgroundService(
        IServiceProvider provider,
        IOptions<SmtpOptions> smtpOptions
    )
    {
        _provider = provider;
        _smtpOptions = smtpOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SendEmails(stoppingToken);
    }

    private async Task SendEmails(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var emailSenderService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
            try
            {
                await emailSenderService.SendEmailsAsync();
            }
            catch (Exception ex)
            {
                // Log the error (consider using a logging framework)
                Console.WriteLine($"Error sending email: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(300), stoppingToken); // Pass cancellation token
        }
    }

}