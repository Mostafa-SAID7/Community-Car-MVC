namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class ExportUserDataRequest
{
    public Guid UserId { get; set; }
    public List<string> DataCategories { get; set; } = new();
    public string Format { get; set; } = "JSON"; 
}