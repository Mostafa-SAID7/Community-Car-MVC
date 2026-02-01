using CommunityCar.Application.Features.Account.ViewModels.Authentication;

namespace CommunityCar.Application.Features.Account.ViewModels.Security;

public class SecurityVM
{
    public SecurityOverviewVM Overview { get; set; } = new();
    public List<ActiveSessionVM> ActiveSessions { get; set; } = new();
}