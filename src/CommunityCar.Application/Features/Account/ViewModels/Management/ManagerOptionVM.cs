namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class ManagerOptionVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public int ManagedUsersCount { get; set; }
    public bool IsAvailable { get; set; }
}