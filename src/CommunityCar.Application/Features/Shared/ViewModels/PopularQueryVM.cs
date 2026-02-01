namespace CommunityCar.Application.Features.Shared.ViewModels;

public class PopularQueryVM
{
    public string Query { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastSearched { get; set; }
}