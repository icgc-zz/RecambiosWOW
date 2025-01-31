namespace RecambiosWOW.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string recipient, string subject, string body);
    Task SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken);
    Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body);
    Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken cancellationToken);
}