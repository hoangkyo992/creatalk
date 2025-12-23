using Common.Application.Configurations;
using Common.Application.Services;
using Common.Infrastructure.ExternalServices;

namespace Common.Infrastructure.Services;

public class SendEmailService(ISendMailApiClient sendMailApiClient, IOptions<EmailServiceOptions> options) : ISendEmailService
{
    private readonly ISendMailApiClient _sendMailApiClient = sendMailApiClient ?? throw new ArgumentNullException(nameof(sendMailApiClient));
    private readonly EmailServiceOptions _emailServiceOptions = options.Value ?? throw new ArgumentNullException(nameof(sendMailApiClient));

    public async Task SendResetPasswordEmail(string applicationName, string subject, string toEmail, string username, string link)
    {
        var sendEmailRequest = MailContentHelper.BuildRequest(_emailServiceOptions.FromAddress, toEmail, subject, username);
        await _sendMailApiClient.SendEmailAsync(sendEmailRequest);
    }
}