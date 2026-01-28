using CommunityCar.Application.Common.Interfaces.Services.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace CommunityCar.Infrastructure.Services.Communication;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var subject = "Confirm Your Email - CommunityCar";
        var body = $@"
            <html>
            <body>
                <h2>Welcome to CommunityCar!</h2>
                <p>Thank you for registering with us. Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Confirm Email</a></p>
                <p>If the button doesn't work, copy and paste this link into your browser:</p>
                <p>{confirmationLink}</p>
                <p>This link will expire in 24 hours.</p>
                <br>
                <p>Best regards,<br>The CommunityCar Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string resetLink)
    {
        var subject = "Reset Your Password - CommunityCar";
        var body = $@"
            <html>
            <body>
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Click the link below to reset it:</p>
                <p><a href='{resetLink}' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                <p>If the button doesn't work, copy and paste this link into your browser:</p>
                <p>{resetLink}</p>
                <p>This link will expire in 1 hour.</p>
                <p>If you didn't request this password reset, please ignore this email.</p>
                <br>
                <p>Best regards,<br>The CommunityCar Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string fullName)
    {
        var subject = "Welcome to CommunityCar!";
        var body = $@"
            <html>
            <body>
                <h2>Welcome to CommunityCar, {fullName}!</h2>
                <p>Your email has been confirmed and your account is now active.</p>
                <p>You can now:</p>
                <ul>
                    <li>Connect with other car enthusiasts</li>
                    <li>Share your car stories and experiences</li>
                    <li>Ask questions and get answers from the community</li>
                    <li>Discover local car events and meetups</li>
                </ul>
                <p><a href='#' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Get Started</a></p>
                <br>
                <p>Happy driving!<br>The CommunityCar Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendTwoFactorTokenAsync(string email, string token)
    {
        var subject = "Your Two-Factor Authentication Code - CommunityCar";
        var body = $@"
            <html>
            <body>
                <h2>Two-Factor Authentication Code</h2>
                <p>Your verification code is:</p>
                <div style='background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; border-radius: 5px; margin: 20px 0;'>
                    {token}
                </div>
                <p>This code will expire in 5 minutes.</p>
                <p>If you didn't request this code, please ignore this email and consider changing your password.</p>
                <br>
                <p>Best regards,<br>The CommunityCar Team</p>
            </body>
            </html>";

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
}
