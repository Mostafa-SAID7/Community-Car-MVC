using CommunityCar.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CommunityCar.Web.Controllers.Api;

[ApiController]
[Route("api/errors")]
public class ErrorsApiController : ControllerBase
{
    private readonly IErrorService _errorService;
    private readonly ILogger<ErrorsApiController> _logger;

    public ErrorsApiController(IErrorService errorService, ILogger<ErrorsApiController> logger)
    {
        _errorService = errorService;
        _logger = logger;
    }

    [HttpPost("log")]
    public async Task<IActionResult> LogError([FromBody] ClientErrorRequest request)
    {
        try
        {
            var userId = User?.Identity?.IsAuthenticated == true 
                ? User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value
                : null;

            var additionalContext = JsonSerializer.Serialize(new
            {
                request.Context,
                ClientInfo = new
                {
                    UserAgent = Request.Headers.UserAgent.ToString(),
                    IpAddress = GetClientIpAddress(),
                    Referer = Request.Headers.Referer.ToString(),
                    RequestId = HttpContext.TraceIdentifier
                }
            });

            string errorId;
            
            if (request.Error != null)
            {
                // Create exception from client error
                var exception = new ClientException(request.Error.Message)
                {
                    ClientStack = request.Error.Stack,
                    ErrorType = request.Error.Type,
                    Source = request.Type
                };
                
                errorId = await _errorService.LogErrorAsync(
                    request.Error.Message, 
                    exception, 
                    userId, 
                    Request.Path, 
                    additionalContext);
            }
            else
            {
                errorId = await _errorService.LogErrorAsync(
                    request.Message ?? "Unknown client error",
                    null,
                    userId,
                    Request.Path,
                    additionalContext
                );
            }

            return Ok(new { success = true, errorId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log client error");
            return Ok(new { success = false, error = "Failed to log error" });
        }
    }

    [HttpDelete("{errorId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteError(string errorId)
    {
        try
        {
            var success = await _errorService.DeleteErrorAsync(errorId);
            if (success)
            {
                return Ok(new { success = true, message = "Error deleted successfully" });
            }
            return NotFound(new { success = false, message = "Error not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete error {ErrorId}", errorId);
            return StatusCode(500, new { success = false, message = "Failed to delete error" });
        }
    }

    [HttpPost("{errorId}/resolve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResolveError(string errorId, [FromBody] ResolveErrorRequest request)
    {
        try
        {
            var userId = User.FindFirst("id")?.Value ?? User.Identity?.Name ?? "Unknown";
            var success = await _errorService.ResolveErrorAsync(errorId, userId, request.Resolution);
            
            if (success)
            {
                return Ok(new { success = true, message = "Error resolved successfully" });
            }
            return NotFound(new { success = false, message = "Error not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve error {ErrorId}", errorId);
            return StatusCode(500, new { success = false, message = "Failed to resolve error" });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetErrorStats()
    {
        try
        {
            var stats = await _errorService.GetErrorStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get error stats");
            return StatusCode(500, new { error = "Failed to get error stats" });
        }
    }

    private string GetClientIpAddress()
    {
        var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        }
        return ipAddress ?? "Unknown";
    }
}

public class ClientErrorRequest
{
    public string? Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Message { get; set; }
    public ClientError? Error { get; set; }
    public Dictionary<string, object>? Context { get; set; }
}

public class ClientError
{
    public string Message { get; set; } = string.Empty;
    public string? Stack { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class ResolveErrorRequest
{
    public string? Resolution { get; set; }
}

public class ClientException : Exception
{
    public string? ClientStack { get; set; }
    public string? ErrorType { get; set; }

    public ClientException(string message) : base(message) { }
    public ClientException(string message, Exception innerException) : base(message, innerException) { }
}