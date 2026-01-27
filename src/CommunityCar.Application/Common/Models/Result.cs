namespace CommunityCar.Application.Common.Models;

public class Result
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public object? Data { get; set; }

    public static Result Success(string message = "", object? data = null)
    {
        return new Result 
        { 
            Succeeded = true, 
            Message = message,
            Data = data
        };
    }

    public static Result Failure(string message, List<string>? errors = null)
    {
        return new Result 
        { 
            Succeeded = false, 
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result 
        { 
            Succeeded = false, 
            Message = "Operation failed",
            Errors = errors.ToList() 
        };
    }
}



