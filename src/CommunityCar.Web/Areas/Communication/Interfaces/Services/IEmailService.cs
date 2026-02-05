namespace CommunityCar.Web.Areas.Communication.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string confirmationLink);
    Task SendPasswordResetAsync(string email, string resetLink);
    Task SendWelcomeEmailAsync(string email, string fullName);
    Task SendLoginConfirmationAsync(string email, string fullName, string ipAddress, string userAgent);
    Task SendTwoFactorTokenAsync(string email, string token);
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendNotificationDigestAsync(string email, string userName, IEnumerable<string> notifications);
}

public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
}



