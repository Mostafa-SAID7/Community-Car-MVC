namespace CommunityCar.Infrastructure.Configuration.Account.Media;

/// <summary>
/// Media and file upload configuration settings
/// </summary>
public class MediaSettings
{
    public const string SectionName = "Media";

    public ImageSettings Images { get; set; } = new();
    public VideoSettings Videos { get; set; } = new();
    public DocumentSettings Documents { get; set; } = new();
    public GallerySettings Gallery { get; set; } = new();
    public StorageSettings Storage { get; set; } = new();
}

/// <summary>
/// Image upload and processing settings
/// </summary>
public class ImageSettings
{
    public bool EnableImageUploads { get; set; } = true;
    public long MaxImageSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public string[] AllowedImageFormats { get; set; } = { "jpg", "jpeg", "png", "gif", "webp", "bmp" };
    public bool EnableImageCompression { get; set; } = true;
    public int ImageCompressionQuality { get; set; } = 85; // 0-100
    public bool EnableImageResizing { get; set; } = true;
    public int MaxImageWidth { get; set; } = 2048;
    public int MaxImageHeight { get; set; } = 2048;
    public bool GenerateThumbnails { get; set; } = true;
    public int ThumbnailWidth { get; set; } = 300;
    public int ThumbnailHeight { get; set; } = 300;
    public bool EnableImageWatermark { get; set; } = false;
    public string WatermarkText { get; set; } = "CommunityCar";
    public bool EnableImageMetadataStripping { get; set; } = true;
    public bool EnableImageVirusScanning { get; set; } = false;
    public int MaxImagesPerUpload { get; set; } = 10;
    public int MaxImagesPerUser { get; set; } = 1000;
}

/// <summary>
/// Video upload and processing settings
/// </summary>
public class VideoSettings
{
    public bool EnableVideoUploads { get; set; } = false;
    public long MaxVideoSize { get; set; } = 100 * 1024 * 1024; // 100MB
    public string[] AllowedVideoFormats { get; set; } = { "mp4", "avi", "mov", "wmv", "flv", "webm" };
    public bool EnableVideoCompression { get; set; } = true;
    public bool EnableVideoThumbnails { get; set; } = true;
    public int VideoThumbnailWidth { get; set; } = 640;
    public int VideoThumbnailHeight { get; set; } = 360;
    public TimeSpan MaxVideoDuration { get; set; } = TimeSpan.FromMinutes(10);
    public bool EnableVideoTranscoding { get; set; } = false;
    public string[] TranscodingFormats { get; set; } = { "mp4", "webm" };
    public bool EnableVideoStreaming { get; set; } = false;
    public bool EnableVideoVirusScanning { get; set; } = false;
    public int MaxVideosPerUpload { get; set; } = 5;
    public int MaxVideosPerUser { get; set; } = 100;
}

/// <summary>
/// Document upload settings
/// </summary>
public class DocumentSettings
{
    public bool EnableDocumentUploads { get; set; } = true;
    public long MaxDocumentSize { get; set; } = 25 * 1024 * 1024; // 25MB
    public string[] AllowedDocumentFormats { get; set; } = { "pdf", "doc", "docx", "xls", "xlsx", "ppt", "pptx", "txt", "rtf" };
    public bool EnableDocumentPreview { get; set; } = false;
    public bool EnableDocumentVirusScanning { get; set; } = true;
    public bool EnableDocumentIndexing { get; set; } = false;
    public bool EnableDocumentVersioning { get; set; } = false;
    public int MaxDocumentVersions { get; set; } = 5;
    public int MaxDocumentsPerUpload { get; set; } = 5;
    public int MaxDocumentsPerUser { get; set; } = 500;
    public TimeSpan DocumentRetentionPeriod { get; set; } = TimeSpan.FromDays(365);
}

/// <summary>
/// User gallery settings
/// </summary>
public class GallerySettings
{
    public bool EnableUserGallery { get; set; } = true;
    public int MaxGalleryItems { get; set; } = 500;
    public bool EnableGalleryOrganization { get; set; } = true;
    public bool EnableGalleryTags { get; set; } = true;
    public int MaxTagsPerItem { get; set; } = 10;
    public bool EnableGallerySharing { get; set; } = true;
    public bool EnableGalleryComments { get; set; } = true;
    public bool EnableGalleryLikes { get; set; } = true;
    public bool EnablePrivateGallery { get; set; } = true;
    public bool EnableGallerySlideshow { get; set; } = true;
    public bool EnableGalleryDownload { get; set; } = false;
    public bool EnableGalleryBulkOperations { get; set; } = true;
    public int GalleryItemsPerPage { get; set; } = 24;
    public bool EnableGallerySearch { get; set; } = true;
}

/// <summary>
/// Media storage settings
/// </summary>
public class StorageSettings
{
    public string StorageProvider { get; set; } = "Local"; // Local, Azure, AWS, Google
    public string LocalStoragePath { get; set; } = "wwwroot/uploads";
    public string BaseUrl { get; set; } = "/uploads";
    public bool EnableCdn { get; set; } = false;
    public string CdnUrl { get; set; } = string.Empty;
    public bool EnableCloudBackup { get; set; } = false;
    public TimeSpan BackupInterval { get; set; } = TimeSpan.FromDays(1);
    public bool EnableStorageQuota { get; set; } = true;
    public long DefaultUserQuota { get; set; } = 1024 * 1024 * 1024; // 1GB
    public long PremiumUserQuota { get; set; } = 10L * 1024 * 1024 * 1024; // 10GB
    public bool EnableStorageAnalytics { get; set; } = true;
    public bool EnableAutomaticCleanup { get; set; } = true;
    public TimeSpan OrphanedFileCleanupInterval { get; set; } = TimeSpan.FromDays(7);
    public TimeSpan DeletedFileRetentionPeriod { get; set; } = TimeSpan.FromDays(30);
    
    // Cloud storage specific settings
    public CloudStorageSettings Azure { get; set; } = new();
    public CloudStorageSettings AWS { get; set; } = new();
    public CloudStorageSettings Google { get; set; } = new();
}

/// <summary>
/// Cloud storage provider settings
/// </summary>
public class CloudStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "media";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool EnablePublicAccess { get; set; } = true;
    public bool EnableEncryption { get; set; } = false;
    public string EncryptionKey { get; set; } = string.Empty;
}