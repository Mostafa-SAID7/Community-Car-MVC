namespace CommunityCar.Domain.Rules;

/// <summary>
/// Interface for business rules that can be validated
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// Gets the error message when the rule is broken
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Checks if the business rule is broken
    /// </summary>
    /// <returns>True if the rule is broken, false otherwise</returns>
    Task<bool> IsBrokenAsync();
}
