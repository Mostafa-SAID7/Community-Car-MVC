using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.BackgroundJobs;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.BackgroundJobs;

/// <summary>
/// Background job service for email operations
/// </summary>
public class EmailBackgroundJobService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailBackgroundJobService> _logger;

    public EmailBackgroundJobService(
        IEmailService emailService,
        ILogger<EmailBackgroundJobService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Send email confirmation in background
    /// </summary>
    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        try
        {
            _logger.LogInformation("Sending email confirmation to {Email}", email);
            await _emailService.SendEmailConfirmationAsync(email, confirmationLink);
            _logger.LogInformation("Email confirmation sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email confirmation to {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Send password reset email in background
    /// </summary>
    public async Task SendPasswordResetAsync(string email, string resetLink)
    {
        try
        {
            _logger.LogInformation("Sending password reset email to {Email}", email);
            await _emailService.SendPasswordResetAsync(email, resetLink);
            _logger.LogInformation("Password reset email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Send welcome email in background
    /// </summary>
    public async Task SendWelcomeEmailAsync(string email, string userName)
    {
        try
        {
            _logger.LogInformation("Sending welcome email to {Email}", email);
            await _emailService.SendWelcomeEmailAsync(email, userName);
            _logger.LogInformation("Welcome email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Send notification digest email in background
    /// </summary>
    public async Task SendNotificationDigestAsync(string email, string userName, List<string> notifications)
    {
        try
        {
            _logger.LogInformation("Sending notification digest to {Email} with {Count} notifications", email, notifications.Count);
            await _emailService.SendNotificationDigestAsync(email, userName, notifications);
            _logger.LogInformation("Notification digest sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification digest to {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Send batch emails in background
    /// </summary>
    public async Task SendBatchEmailsAsync(List<(string Email, string Subject, string Body)> emails)
    {
        try
        {
            _logger.LogInformation("Sending batch emails: {Count} emails", emails.Count);
            
            var tasks = emails.Select(async email =>
            {
                try
                {
                    await _emailService.SendEmailAsync(email.Email, email.Subject, email.Body);
                    _logger.LogDebug("Batch email sent to {Email}", email.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send batch email to {Email}", email.Email);
                }
            });

            await Task.WhenAll(tasks);
            _logger.LogInformation("Batch emails processing completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process batch emails");
            throw;
        }
    }
}


