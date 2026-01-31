using CommunityCar.Application.Common.Interfaces.Services.Communication;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CommunityCar.Infrastructure.Services.Communication;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly ILogger<EmailTemplateService> _logger;
    private readonly Dictionary<string, string> _templateCache = new();
    private readonly Assembly _assembly;

    public EmailTemplateService(ILogger<EmailTemplateService> logger)
    {
        _logger = logger;
        _assembly = Assembly.GetExecutingAssembly();
    }

    public async Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> replacements)
    {
        try
        {
            var template = await LoadTemplateAsync(templateName);
            return ProcessTemplate(template, replacements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load email template: {TemplateName}", templateName);
            return GetFallbackTemplate(templateName, replacements);
        }
    }

    private async Task<string> LoadTemplateAsync(string templateName)
    {
        if (_templateCache.TryGetValue(templateName, out var cachedTemplate))
        {
            return cachedTemplate;
        }

        var resourceName = $"CommunityCar.Infrastructure.Templates.Email.{templateName}.html";
        
        using var stream = _assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Email template not found: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        var template = await reader.ReadToEndAsync();
        _templateCache[templateName] = template;
        
        return template;
    }

    private static string ProcessTemplate(string template, Dictionary<string, string> replacements)
    {
        var processedTemplate = template;
        
        foreach (var replacement in replacements)
        {
            processedTemplate = processedTemplate.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
        }
        
        return processedTemplate;
    }

    private static string GetFallbackTemplate(string templateName, Dictionary<string, string> replacements)
    {
        return templateName switch
        {
            "EmailConfirmation" => $@"
                <html><body>
                    <h2>Confirm Your Email - CommunityCar</h2>
                    <p>Please confirm your email by clicking: <a href='{replacements.GetValueOrDefault("CONFIRMATION_LINK", "#")}'>Confirm Email</a></p>
                    <p>Best regards,<br>The CommunityCar Team</p>
                </body></html>",
            
            "PasswordReset" => $@"
                <html><body>
                    <h2>Password Reset - CommunityCar</h2>
                    <p>Reset your password by clicking: <a href='{replacements.GetValueOrDefault("RESET_LINK", "#")}'>Reset Password</a></p>
                    <p>Best regards,<br>The CommunityCar Team</p>
                </body></html>",
            
            "LoginConfirmation" => $@"
                <html><body>
                    <h2>Login Confirmation - CommunityCar</h2>
                    <p>Hello {replacements.GetValueOrDefault("FULL_NAME", "User")},</p>
                    <p>Your account was accessed at {replacements.GetValueOrDefault("LOGIN_TIME", "unknown time")} from {replacements.GetValueOrDefault("IP_ADDRESS", "unknown IP")}.</p>
                    <p>Best regards,<br>The CommunityCar Team</p>
                </body></html>",
            
            "Welcome" => $@"
                <html><body>
                    <h2>Welcome to CommunityCar!</h2>
                    <p>Welcome {replacements.GetValueOrDefault("FULL_NAME", "User")}!</p>
                    <p>Your account is now active. <a href='{replacements.GetValueOrDefault("GET_STARTED_LINK", "#")}'>Get Started</a></p>
                    <p>Best regards,<br>The CommunityCar Team</p>
                </body></html>",
            
            "TwoFactorToken" => $@"
                <html><body>
                    <h2>Two-Factor Authentication Code</h2>
                    <p>Your verification code is: <strong>{replacements.GetValueOrDefault("TOKEN", "000000")}</strong></p>
                    <p>This code expires in 5 minutes.</p>
                    <p>Best regards,<br>The CommunityCar Team</p>
                </body></html>",
            
            _ => $@"
                <html><body>
                    <h2>CommunityCar Notification</h2>
                    <p>This is a notification from CommunityCar.</p>
                    <p>Best regards,<br>The CommunityCar Team</p>
                </body></html>"
        };
    }
}