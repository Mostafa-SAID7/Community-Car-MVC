namespace CommunityCar.Domain.Enums.Account;

public enum LockoutType
{
    Temporary = 0,
    Permanent = 1,
    Suspicious = 2,
    Administrative = 3,
    Security = 4
}