namespace CommunityCar.Application.Common.Interfaces.Services.Account.Core;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}