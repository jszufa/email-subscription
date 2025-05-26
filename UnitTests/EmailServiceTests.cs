using EmailSubscription.Api;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;

namespace UnitTests;

public class EmailServiceTests
{
    private readonly EmailService _sut;
    private readonly Mock<IOptions<EmailSettings>> _optionsMock;
    private readonly EmailSettings _emailSettings = new EmailSettings
    {
        User = "test@test.com",
        Password = "test",
        SmtpHost = "smtp.example.com",
        SmtpPort = 587,
        SenderName = "test"
    };
    
    public EmailServiceTests()
    {
        _optionsMock = new Mock<IOptions<EmailSettings>>();
        _optionsMock.Setup(o => o.Value).Returns(_emailSettings);
        _sut = new EmailService(_optionsMock.Object);
    }
    
    [Fact]
    public async Task SendEmailAsync_WhenValidInput_ShouldSendEmail()
    {
        // arrange
        var receiverEmail = "recipient@example.com";
        var receiverName = "Jack Jones";
        var subject = "Welcome to Our Service";
        var body = "Thank you for signing up!";
        var smtpMock = new Mock<ISmtpClient>();

        CreateConfiguredSmtpMock(smtpMock, receiverEmail, receiverName, subject, body);

        // act
        await _sut.SendEmailAsync(receiverEmail, receiverName, subject, body, smtpMock.Object);
        
        // assert
        smtpMock.Verify(x => x.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, It.IsAny<SecureSocketOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        smtpMock.Verify(x => x.AuthenticateAsync(_emailSettings.User, _emailSettings.Password, It.IsAny<CancellationToken>()), Times.Once());
        smtpMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Once());
        smtpMock.Verify(x => x.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once());
        
    }

    private void CreateConfiguredSmtpMock(Mock<ISmtpClient> smtpMock, string receiverEmail, string receiverName, string subject,
        string body)
    {
        smtpMock.Setup(s => s.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, It.IsAny<SecureSocketOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        smtpMock.Setup(s => s.AuthenticateAsync(_emailSettings.User, _emailSettings.Password, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        smtpMock.Setup(s =>
                s.SendAsync(It.Is<MimeMessage>(m=> VerifyMimeMessage(m, receiverEmail, receiverName, subject, body)), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
            .ReturnsAsync("some-id");

        smtpMock.Setup(s => s.DisconnectAsync(true, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private bool VerifyMimeMessage(MimeMessage message, string receiverEmail, string receiverName, string subject, string body)
    {
        return message.To.Any(t => t.ToString() == $"{receiverName} <{receiverEmail}>") &&
               message.From.Any(f => f.ToString() == $"{_emailSettings.SenderName} <{_emailSettings.User}>") &&
               message.Subject == subject &&
               message.Body.ToString().Contains(body);
    }
}