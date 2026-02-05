using Microsoft.AspNetCore.Http;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Media;

public class UploadImageRequest
{
    public Guid UserId { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string? ImageData { get; set; } // Base64
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; } = true;
    public string? Category { get; set; }
}
