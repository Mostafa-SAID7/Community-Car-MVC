using Microsoft.AspNetCore.Mvc.Filters;
using System.IO.Compression;

namespace CommunityCar.Web.Attributes;

/// <summary>
/// Compresses response content using Gzip or Deflate
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CompressVMAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Minimum response size in bytes to trigger compression
    /// </summary>
    public int MinimumSizeBytes { get; set; } = 1024; // 1KB

    /// <summary>
    /// Content types to compress
    /// </summary>
    public string[] ContentTypes { get; set; } = 
    {
        "text/html",
        "text/css",
        "text/javascript",
        "application/javascript",
        "application/json",
        "text/json",
        "application/xml",
        "text/xml",
        "text/plain"
    };

    /// <summary>
    /// Compression level
    /// </summary>
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var response = context.HttpContext.Response;
        var request = context.HttpContext.Request;

        // Check if client accepts compression
        var acceptEncoding = request.Headers.AcceptEncoding.ToString().ToLowerInvariant();
        if (string.IsNullOrEmpty(acceptEncoding))
        {
            base.OnActionExecuted(context);
            return;
        }

        // Check content type
        var contentType = response.ContentType?.ToLowerInvariant();
        if (string.IsNullOrEmpty(contentType) || !ContentTypes.Any(ct => contentType.Contains(ct)))
        {
            base.OnActionExecuted(context);
            return;
        }

        // Determine compression method
        string? compressionMethod = null;
        if (acceptEncoding.Contains("gzip"))
        {
            compressionMethod = "gzip";
        }
        else if (acceptEncoding.Contains("deflate"))
        {
            compressionMethod = "deflate";
        }

        if (compressionMethod == null)
        {
            base.OnActionExecuted(context);
            return;
        }

        // Set up compression
        Stream compressionStream;
        switch (compressionMethod)
        {
            case "gzip":
                compressionStream = new GZipStream(response.Body, CompressionLevel);
                response.Headers.Append("Content-Encoding", "gzip");
                break;
            case "deflate":
                compressionStream = new DeflateStream(response.Body, CompressionLevel);
                response.Headers.Append("Content-Encoding", "deflate");
                break;
            default:
                base.OnActionExecuted(context);
                return;
        }

        // Replace response body stream
        var originalStream = response.Body;
        response.Body = compressionStream;

        // Clean up when response is complete
        response.OnCompleted(async () =>
        {
            await compressionStream.DisposeAsync();
            response.Body = originalStream;
        });

        // Add Vary header
        response.Headers.Append("Vary", "Accept-Encoding");

        base.OnActionExecuted(context);
    }
}

/// <summary>
/// Disables compression for specific actions
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class NoCompressionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        // Remove any compression headers that might have been set
        context.HttpContext.Response.Headers.Remove("Content-Encoding");
        
        base.OnActionExecuted(context);
    }
}

/// <summary>
/// Compresses only large responses
/// </summary>
public class CompressLargeVMsAttribute : CompressVMAttribute
{
    public CompressLargeVMsAttribute()
    {
        MinimumSizeBytes = 10240; // 10KB
        CompressionLevel = CompressionLevel.Fastest; // Faster compression for large responses
    }
}

/// <summary>
/// High compression for static content
/// </summary>
public class HighCompressionAttribute : CompressVMAttribute
{
    public HighCompressionAttribute()
    {
        MinimumSizeBytes = 512; // 512 bytes
        CompressionLevel = CompressionLevel.SmallestSize;
        ContentTypes = new[]
        {
            "text/css",
            "text/javascript",
            "application/javascript",
            "text/html",
            "application/json",
            "text/json"
        };
    }
}