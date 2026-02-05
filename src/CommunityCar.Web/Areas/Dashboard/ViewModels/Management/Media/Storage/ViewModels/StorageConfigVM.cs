namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.Storage.ViewModels;

public class StorageConfigVM
{
    public string StorageProvider { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
    public List<string> AllowedFileTypes { get; set; } = new();
    public bool EnableCompression { get; set; }
}




