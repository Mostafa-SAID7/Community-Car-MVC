using AutoMapper;
using CommunityCar.Application.Features.Account.Mappings.Core;
using CommunityCar.Application.Features.Account.Mappings.Authentication;
using CommunityCar.Application.Features.Account.Mappings.Activity;
using CommunityCar.Application.Features.Account.Mappings.Gamification;
using CommunityCar.Application.Features.Account.Mappings.Social;
using CommunityCar.Application.Features.Account.Mappings.Media;

namespace CommunityCar.Application.Features.Account.Mappings;

/// <summary>
/// Umbrella mapping profile that includes all account-related mapping profiles.
/// This provides backward compatibility while maintaining the new organized structure.
/// </summary>
public class AccountMappingProfile : AutoMapper.Profile
{
    public AccountMappingProfile()
    {
        // Include all the organized mapping profiles
        // Note: AutoMapper will automatically discover and include profiles from the same assembly
        // This class serves as documentation and potential future configuration point
    }
}