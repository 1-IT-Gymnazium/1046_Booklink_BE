using BooklinkBE.API.Services.Implementations;

namespace BooklinkBE.API.BackgroundWorkers;

// Runs in the background and sends emails every 60 seconds using EmailSenderService.
public class EmailSenderBackgroundService(
    IServiceProvider provider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SendEmails(stoppingToken);
    }

    private async Task SendEmails(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = provider.CreateScope();
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

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}