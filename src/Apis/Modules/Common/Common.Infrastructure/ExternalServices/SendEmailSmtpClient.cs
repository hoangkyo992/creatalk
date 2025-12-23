using System.Net;
using System.Net.Mail;
using Common.Application.Configurations;

namespace Common.Infrastructure.ExternalServices;

public class SendEmailSmtpClient : ISendMailApiClient
{
    private readonly MailAddress _sender;
    private readonly SmtpClient _smtpClient;

    public SendEmailSmtpClient(IOptions<EmailServiceOptions> options)
    {
        var emailServiceOptions = options.Value;
        _sender = new MailAddress(emailServiceOptions.FromAddress, emailServiceOptions.FromDisplayName);
        _smtpClient = new SmtpClient
        {
            Host = emailServiceOptions.Host,
            Port = emailServiceOptions.Port,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_sender.Address, emailServiceOptions.AppPassword)
        };
    }

    public async Task SendEmailAsync(object content, CancellationToken cancellationToken = default)
    {
        var mailContent = MailContentHelper.GetRequest(content);
        var toAddress = new MailAddress(mailContent.ToEmail, string.Empty);
        using (var message = new MailMessage(_sender, toAddress)
        {
            Subject = mailContent.Subject,
            Body = mailContent.Content,
            IsBodyHtml = true
        })
        {
            await _smtpClient.SendMailAsync(message);
        }
    }
}