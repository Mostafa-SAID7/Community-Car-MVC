namespace CommunityCar.Application.Common.Models;

public class AuthResult
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public object? Data { get; set; }

    public static AuthResult Success(string message = "", object? data = null)
    {
        return new AuthResult
        {
            Succeeded = true,
            Message = message,
            Data = data
        };
    }

    public static AuthResult Failure(string message, List<string>? errors = null)
    {
        return new AuthResult
        {
            Succeeded = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static AuthResult Failure(List<string> errors)
    {
        return new AuthResult
        {
            Succeeded = false,
            Message = "Operation failed",
            Errors = errors
        };
    }
}