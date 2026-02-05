using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Core;

/*
 * USER CORE VIEWMODELS REFERENCE FILE
 * ===================================
 * 
 * This file originally contained 8 ViewModels that have been split into separate files for better organization.
 * All ViewModels are now located in individual files within this same directory.
 * 
 * SPLIT FILES:
 * ============
 * 
 * Core ViewModels:
 * - ProfileVM.cs
 * - UserStatisticsVM.cs
 * - ProfileStatsVM.cs
 * - ProfileSettingsVM.cs
 * - UpdateProfileVM.cs
 * - UserCardVM.cs
 * - AccountIdentityVM.cs
 * - AccountClaimVM.cs
 * 
 * ALIASES:
 * ========
 * The following aliases are maintained for backward compatibility:
 * - ProfileIndexVM : ProfileVM
 * - UpdateProfileRequest : UpdateProfileVM
 * - UserIdentityVM : AccountIdentityVM
 * - UserClaimVM : AccountClaimVM
 * 
 * USAGE:
 * ======
 * Import individual ViewModels as needed:
 * using CommunityCar.Application.Features.Account.ViewModels.Core;
 * 
 * All ViewModels maintain the same namespace and functionality as before.
 * This split improves code organization, maintainability, and reduces file size.
 */

// Aliases for compatibility
public class ProfileIndexVM : ProfileVM { }
public class UpdateProfileRequest : UpdateProfileVM { }
public class UserIdentityVM : AccountIdentityVM { }
public class UserClaimVM : AccountClaimVM { }
