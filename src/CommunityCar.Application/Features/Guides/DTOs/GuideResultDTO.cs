namespace CommunityCar.Application.Features.Guides.DTOs;

public class GuideResultDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? GuideId { get; set; }
}