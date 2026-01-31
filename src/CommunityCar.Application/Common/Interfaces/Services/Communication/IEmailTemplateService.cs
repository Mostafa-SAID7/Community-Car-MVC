namespace CommunityCar.Application.Common.Interfaces.Services.Communication;

public interface IEmailTemplateService
{
    Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> replacements);
}