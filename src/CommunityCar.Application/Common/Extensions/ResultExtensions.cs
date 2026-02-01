using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Common.Extensions;

/// <summary>
/// Extensions for Result pattern operations
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps a successful result to a new type
    /// </summary>
    public static Result<TNew> Map<T, TNew>(this Result<T> result, Func<T, TNew> mapper)
    {
        return result.IsSuccess
            ? Result<TNew>.Success(mapper(result.Value))
            : Result<TNew>.Failure(result.Errors);
    }

    /// <summary>
    /// Maps a successful result asynchronously
    /// </summary>
    public static async Task<Result<TNew>> MapAsync<T, TNew>(
        this Result<T> result,
        Func<T, Task<TNew>> mapper)
    {
        return result.IsSuccess
            ? Result<TNew>.Success(await mapper(result.Value))
            : Result<TNew>.Failure(result.Errors);
    }

    /// <summary>
    /// Binds a result to another operation that returns a result
    /// </summary>
    public static Result<TNew> Bind<T, TNew>(this Result<T> result, Func<T, Result<TNew>> binder)
    {
        return result.IsSuccess
            ? binder(result.Value)
            : Result<TNew>.Failure(result.Errors);
    }

    /// <summary>
    /// Binds a result asynchronously
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Result<T> result,
        Func<T, Task<Result<TNew>>> binder)
    {
        return result.IsSuccess
            ? await binder(result.Value)
            : Result<TNew>.Failure(result.Errors);
    }

    /// <summary>
    /// Executes an action if the result is successful
    /// </summary>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Executes an action if the result is a failure
    /// </summary>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<IEnumerable<string>> action)
    {
        if (result.IsFailure)
        {
            action(result.Errors);
        }
        return result;
    }

    /// <summary>
    /// Executes an async action if the result is successful
    /// </summary>
    public static async Task<Result<T>> OnSuccessAsync<T>(
        this Result<T> result,
        Func<T, Task> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Executes an async action if the result is a failure
    /// </summary>
    public static async Task<Result<T>> OnFailureAsync<T>(
        this Result<T> result,
        Func<IEnumerable<string>, Task> action)
    {
        if (result.IsFailure)
        {
            await action(result.Errors);
        }
        return result;
    }

    /// <summary>
    /// Combines multiple results into a single result
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToList();
        
        if (!failures.Any())
            return Result.Success();

        var allErrors = failures.SelectMany(f => f.Errors).ToList();
        return Result.Failure(allErrors);
    }

    /// <summary>
    /// Combines multiple results with values
    /// </summary>
    public static Result<IEnumerable<T>> Combine<T>(params Result<T>[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToList();
        
        if (!failures.Any())
        {
            var values = results.Select(r => r.Value).ToList();
            return Result<IEnumerable<T>>.Success(values);
        }

        var allErrors = failures.SelectMany(f => f.Errors).ToList();
        return Result<IEnumerable<T>>.Failure(allErrors);
    }

    /// <summary>
    /// Converts a nullable value to a Result
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, string errorMessage = "Value is null")
        where T : class
    {
        return value != null
            ? Result<T>.Success(value)
            : Result<T>.Failure(errorMessage);
    }

    /// <summary>
    /// Converts a nullable struct to a Result
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, string errorMessage = "Value is null")
        where T : struct
    {
        return value.HasValue
            ? Result<T>.Success(value.Value)
            : Result<T>.Failure(errorMessage);
    }

    /// <summary>
    /// Ensures a condition is met, otherwise returns failure
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value)
            ? result
            : Result<T>.Failure(errorMessage);
    }

    /// <summary>
    /// Filters a result based on a predicate
    /// </summary>
    public static Result<T> Where<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
    {
        return result.Ensure(predicate, errorMessage);
    }

    /// <summary>
    /// Converts Result to Task<Result>
    /// </summary>
    public static Task<Result<T>> AsTask<T>(this Result<T> result)
    {
        return Task.FromResult(result);
    }

    /// <summary>
    /// Converts Result to Task<Result>
    /// </summary>
    public static Task<Result> AsTask(this Result result)
    {
        return Task.FromResult(result);
    }

    /// <summary>
    /// Matches the result and executes appropriate function
    /// </summary>
    public static TResult Match<T, TResult>(
        this Result<T> result,
        Func<T, TResult> onSuccess,
        Func<IEnumerable<string>, TResult> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Errors);
    }

    /// <summary>
    /// Matches the result and executes appropriate async function
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Result<T> result,
        Func<T, Task<TResult>> onSuccess,
        Func<IEnumerable<string>, Task<TResult>> onFailure)
    {
        return result.IsSuccess
            ? await onSuccess(result.Value)
            : await onFailure(result.Errors);
    }
}