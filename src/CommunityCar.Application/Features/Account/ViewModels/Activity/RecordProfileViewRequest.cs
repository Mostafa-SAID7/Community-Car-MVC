namespace CommunityCar.Application.Features.Account.ViewModels.Activity;

public class RecordProfileViewRequest
{
    public Guid ProfileUserId { get; set; }
    public Guid? ViewerId { get; set; }
    public bool IsAnonymous { get; set; }
    public string? ViewSource { get; set; }
    public string? Device { get; set; }
}