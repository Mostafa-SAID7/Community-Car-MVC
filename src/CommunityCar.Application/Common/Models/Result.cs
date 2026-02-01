namespace CommunityCar.Application.Common.Models;

public class Result
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public object? Data { get; set; }

    public bool IsSuccess => Succeeded;
    public bool IsFailure => !Succeeded;

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

public class Result<T> : Result
{
    public T Value { get; set; } = default!;

    public static Result<T> Success(T value, string message = "")
    {
        return new Result<T>
        {
            Succeeded = true,
            Value = value,
            Message = message
        };
    }

    public static new Result<T> Failure(string message, List<string>? errors = null)
    {
        return new Result<T>
        {
            Succeeded = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static new Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T>
        {
            Succeeded = false,
            Message = "Operation failed",
            Errors = errors.ToList()
        };
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>
        {
            Succeeded = false,
            Message = error,
            Errors = new List<string> { error }
        };
    }
}



public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedResult()
    {
    }

    public PaginatedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}



