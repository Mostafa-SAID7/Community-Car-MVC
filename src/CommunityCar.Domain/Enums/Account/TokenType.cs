namespace CommunityCar.Domain.Enums.Account;

public enum TokenType
{
    EmailVerification = 1,
    PasswordReset = 2,
    SmsVerification = 3,
    EmailTwoFactor = 4,
    SmsTwoFactor = 5,
    PhoneVerification = 6,
    AccountActivation = 7,
    LoginVerification = 8
}