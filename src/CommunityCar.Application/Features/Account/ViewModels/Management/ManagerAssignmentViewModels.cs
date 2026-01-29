namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class AssignManagerVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid? CurrentManagerId { get; set; }
    public string? CurrentManagerName { get; set; }
    public List<ManagerOptionVM> AvailableManagers { get; set; } = new();
}

public class ManagerOptionVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public int ManagedUsersCount { get; set; }
    public bool IsAvailable { get; set; }
}