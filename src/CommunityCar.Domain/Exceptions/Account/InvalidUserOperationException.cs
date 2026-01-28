namespace CommunityCar.Domain.Exceptions.Account;

/// <summary>
/// Exception thrown when an invalid user operation is attempted
/// </summary>
public class InvalidUserOperationException : DomainException
{
    public Guid UserId { get; }
    public string Operation { get; }

    public InvalidUserOperationException(Guid userId, string operation, string message) 
        : base($"Invalid operation '{operation}' for user {userId}: {message}")
    {
        UserId = userId;
        Operation = operation;
    }

    public InvalidUserOperationException(Guid userId, string operation, string message, Exception innerException) 
        : base($"Invalid operation '{operation}' for user {userId}: {message}", innerException)
    {
        UserId = userId;
        Operation = operation;
    }
}