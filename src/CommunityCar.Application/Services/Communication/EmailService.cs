using CommunityCar.Application.Common.Interfaces.Services.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace CommunityCar.Application.Services.Communication;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailTemplateService _templateService;

    public EmailService(
        IConfiguration configuration, 
        ILogger<EmailService> logger,
        IEmailTemplateService templateService)
    {
        _configuration = configuration;
        _logger = logger;
        _templateService = templateService;
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var subject = "Confirm Your Email - CommunityCar";
        var replacements = new Dictionary<string, string>
        {
            { "CONFIRMATION_LINK", confirmationLink }
        };
        
        var body = await _templateService.GetTemplateAsync("EmailConfirmation", replacements);
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string resetLink)
    {
        var subject = "Reset Your Password - CommunityCar";
        var replacements = new Dictionary<string, string>
        {
            { "RESET_LINK", resetLink }
        };
        
        var body = await _templateService.GetTemplateAsync("PasswordReset", replacements);
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendLoginConfirmationAsync(string email, string fullName, string ipAddress, string userAgent)
    {
        var subject = "New Login to Your Account - CommunityCar";
        var loginTime = DateTime.UtcNow.ToString("MMM dd, yyyy 'at' HH:mm UTC");
        var replacements = new Dictionary<string, string>
        {
            { "FULL_NAME", fullName },
            { "LOGIN_TIME", loginTime },
            { "IP_ADDRESS", ipAddress },
            { "USER_AGENT", userAgent }
        };
        
        var body = await _templateService.GetTemplateAsync("LoginConfirmation", replacements);
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string fullName)
    {
        var subject = "Welcome to CommunityCar!";
        var replacements = new Dictionary<string, string>
        {
            { "FULL_NAME", fullName },
            { "GET_STARTED_LINK", GetBaseUrl() + "/feed" }
        };
        
        var body = await _templateService.GetTemplateAsync("Welcome", replacements);
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendTwoFactorTokenAsync(string email, string token)
    {
        var subject = "Your Two-Factor Authentication Code - CommunityCar";
        var replacements = new Dictionary<string, string>
        {
            { "TOKEN", token }
        };
        
        var body = await _templateService.GetTemplateAsync("TwoFactorToken", replacements);
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");
            var smtpHost = smtpSettings["SmtpHost"];
            var smtpPort = int.Parse(smtpSettings["SmtpPort"] ?? "587");
            var smtpUsername = smtpSettings["SmtpUsername"];
            var smtpPassword = smtpSettings["SmtpPassword"];
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"];

            // For development, log email instead of sending
            if (string.IsNullOrEmpty(smtpHost))
            {
                _logger.LogInformation("Email would be sent to {To} with subject: {Subject}", to, subject);
                _logger.LogInformation("Email body: {Body}", body);
                return;
            }

            using var client = new SmtpClient(smtpHost, smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? smtpUsername!, fromName ?? "CommunityCar"),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendNotificationDigestAsync(string email, string userName, IEnumerable<string> notifications)
    {
        // Implementation for sending notification digest
        _logger.LogInformation("Sending notification digest to {Email} for user {UserName}", email, userName);
        await Task.CompletedTask;
    }

    private string GetBaseUrl()
    {
        // Try to get base URL from configuration, fallback to localhost
        return _configuration["BaseUrl"] ?? "https://localhost:5003";
    }
}