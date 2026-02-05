namespace CommunityCar.Application.Features.Dashboard.Management.Media.SoftDelete.ViewModels;

public class RelatedContentVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
