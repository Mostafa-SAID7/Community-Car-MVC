namespace CommunityCar.Application.Common.Interfaces.Services.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}


