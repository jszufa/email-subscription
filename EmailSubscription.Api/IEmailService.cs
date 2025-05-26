using MailKit.Net.Smtp;

namespace EmailSubscription.Api;

public interface IEmailService
{
    public Task SendEmailAsync(string receiverEmail, string receiverName, string subject, string body, ISmtpClient smtpClient);
}