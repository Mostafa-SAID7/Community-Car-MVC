namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class TopContentVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public DateTime CreatedAt { get; set; }
}