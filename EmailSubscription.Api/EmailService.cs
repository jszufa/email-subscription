using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailSubscription.Api;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string receiverEmail, string receiverName, string subject, string body)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.User));
        email.To.Add(new MailboxAddress(receiverName, receiverEmail));

        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = body
        };

        using (var smtp = new SmtpClient())
        {
            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.User, _settings.Password);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}