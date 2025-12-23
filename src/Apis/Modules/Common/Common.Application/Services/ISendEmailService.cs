namespace Common.Application.Services;

public interface ISendEmailService
{
    Task SendResetPasswordEmail(string applicationName, string subject, string toEmail, string username, string link);
}