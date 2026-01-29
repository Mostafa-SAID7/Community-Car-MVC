$f = "c:\Users\memos\OneDrive\Desktop\Projects\Community Car MVC\src\CommunityCar.Application\Services\Account\UserAnalyticsService.cs"
$c = Get-Content $f -Raw -Encoding UTF8
$c = $c.Replace('using CommunityCar.Application.Features.Account.ViewModels;', 'using CommunityCar.Application.Features.Account.ViewModels;`r`nusing CommunityCar.Application.Features.Account.ViewModels.Activity;`r`nusing CommunityCar.Application.Features.Account.ViewModels.Core;`r`nusing CommunityCar.Application.Features.Account.ViewModels.Social;')
$c = $c.Replace('ActivityAnalyticsDTO', 'ActivityAnalyticsVM')
$c = $c.Replace('UserStatisticsDTO', 'UserStatisticsVM')
$c = $c.Replace('FollowingAnalyticsDTO', 'FollowingAnalyticsVM')
$c = $c.Replace('InterestAnalyticsDTO', 'InterestAnalyticsVM')
$c = $c.Replace('ProfileViewAnalyticsDTO', 'ProfileViewStatsVM')
$c = $c.Replace('ProfileViewStatsDTO', 'ProfileViewStatsVM')
$c = $c.Replace('NetworkUserDTO', 'NetworkUserVM')
$c = $c.Replace('TimelineActivityDTO', 'TimelineActivityVM')
$c = $c.Replace('UserSuggestionDTO', 'UserSuggestionVM')
$c = $c.Replace('ProfileInterestDTO', 'ProfileInterestVM')
$c = $c.Replace('ProfileViewDTO', 'ProfileViewVM')
$c = $c.Replace('ProfileViewerDTO', 'ProfileViewerVM')
$c = $c.Replace('ActivityTrendDTO', 'ActivityTrendVM')
Set-Content $f $c -Encoding UTF8
