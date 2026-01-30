using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Entities.Account.Authentication;

public class UserToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public TokenType TokenType { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public string? Purpose { get; private set; } // EmailVerification, PasswordReset, TwoFactor, etc.
    public string? Metadata { get; private set; } // JSON for additional data
    public int AttemptCount { get; private set; }
    public int MaxAttempts { get; private set; } = 3;
    public bool IsActive => IsValid();

    public UserToken(
        Guid userId,
        TokenType tokenType,
        string token,
        TimeSpan validity,
        string? purpose = null,
        int maxAttempts = 3)
    {
        UserId = userId;
        TokenType = tokenType;
        Token = token;
        ExpiresAt = DateTime.UtcNow.Add(validity);
        Purpose = purpose;
        MaxAttempts = maxAttempts;
        IsUsed = false;
        AttemptCount = 0;
    }

    private UserToken() { }

    public bool IsValid()
    {
        return !IsUsed && 
               !IsExpired() && 
               AttemptCount < MaxAttempts;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

    public bool ValidateToken(string providedToken)
    {
        AttemptCount++;
        Audit(UpdatedBy);

        if (!IsValid())
            return false;

        return Token == providedToken;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void SetMetadata(string metadata)
    {
        Metadata = metadata;
        Audit(UpdatedBy);
    }

    public void ExtendValidity(TimeSpan additionalTime)
    {
        if (!IsUsed)
        {
            ExpiresAt = ExpiresAt.Add(additionalTime);
            Audit(UpdatedBy);
        }
    }

    public void Invalidate()
    {
        ExpiresAt = DateTime.UtcNow.AddSeconds(-1);
        Audit(UpdatedBy);
    }
}