namespace CommunityCar.Web.Areas.Communication.Interfaces.Services;

public interface IEmailTemplateService
{
    Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> replacements);
}
