using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

public class MinimumAgeRule : IBusinessRule
{
    private readonly DateTime? _dateOfBirth;

    public MinimumAgeRule(DateTime? dateOfBirth)
    {
        _dateOfBirth = dateOfBirth;
    }

    public string Message => "User must be at least 13 years old to register.";

    public Task<bool> IsBrokenAsync()
    {
        if (!_dateOfBirth.HasValue)
            return Task.FromResult(false); // Optional field

        var age = DateTime.Today.Year - _dateOfBirth.Value.Year;
        if (_dateOfBirth.Value.Date > DateTime.Today.AddYears(-age))
            age--;

        return Task.FromResult(age < 13);
    }
}