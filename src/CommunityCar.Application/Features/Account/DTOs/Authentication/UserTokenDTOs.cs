namespace CommunityCar.Application.Features.Account.DTOs.Authentication;

#region User Token DTOs

public class UserTokenDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class CreateTokenRequest
{
    public Guid UserId { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string? TokenType { get; set; }
}

public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public bool IsExpired { get; set; }
    public UserTokenDTO? Token { get; set; }
    public string? ErrorMessage { get; set; }
}

public class InvalidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public class InvalidateUserTokensRequest
{
    public Guid UserId { get; set; }
    public string? TokenType { get; set; }
}

#endregion