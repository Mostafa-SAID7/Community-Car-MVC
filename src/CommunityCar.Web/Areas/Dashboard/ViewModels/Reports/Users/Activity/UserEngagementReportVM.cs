namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Activity;

/// <summary>
/// ViewModel for user engagement report
/// </summary>
public class UserEngagementReportVM
{
    public int TotalEngagements { get; set; }
    public int PostsCreated { get; set; }
    public int CommentsPosted { get; set; }
    public int LikesGiven { get; set; }
    public int SharesMade { get; set; }
    public int QuestionsAsked { get; set; }
    public int AnswersProvided { get; set; }
    public int ReviewsWritten { get; set; }
    public int StoriesShared { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal AverageEngagementPerUser { get; set; }
    public List<EngagementTrendReportVM> EngagementTrends { get; set; } = new();
    public List<TopEngagedUserVM> TopEngagedUsers { get; set; } = new();
    public List<EngagementTypeDistributionVM> EngagementDistribution { get; set; } = new();
}

/// <summary>
/// ViewModel for engagement trend in reports
/// </summary>
public class EngagementTrendReportVM
{
    public DateTime Date { get; set; }
    public int TotalEngagements { get; set; }
    public int Posts { get; set; }
    public int Comments { get; set; }
    public int Likes { get; set; }
    public int Shares { get; set; }
    public string Period { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for top engaged users in reports
/// </summary>
public class TopEngagedUserVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public int TotalEngagements { get; set; }
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesCount { get; set; }
    public int SharesCount { get; set; }
    public decimal EngagementScore { get; set; }
    public string EngagementLevel { get; set; } = string.Empty; // Low, Medium, High, Very High
}

/// <summary>
/// ViewModel for engagement type distribution
/// </summary>
public class EngagementTypeDistributionVM
{
    public string EngagementType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for user demographics report
/// </summary>
public class UserDemographicsReportVM
{
    public List<AgeGroupDistributionVM> AgeDistribution { get; set; } = new();
    public List<GenderDistributionVM> GenderDistribution { get; set; } = new();
    public List<LocationDistributionVM> LocationDistribution { get; set; } = new();
    public List<InterestDistributionVM> InterestDistribution { get; set; } = new();
    public List<JoinDateDistributionVM> JoinDateDistribution { get; set; } = new();
}

/// <summary>
/// ViewModel for age group distribution
/// </summary>
public class AgeGroupDistributionVM
{
    public string AgeGroup { get; set; } = string.Empty; // 18-24, 25-34, etc.
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// ViewModel for gender distribution
/// </summary>
public class GenderDistributionVM
{
    public string Gender { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// ViewModel for location distribution
/// </summary>
public class LocationDistributionVM
{
    public string Country { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// ViewModel for interest distribution
/// </summary>
public class InterestDistributionVM
{
    public string Interest { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// ViewModel for join date distribution
/// </summary>
public class JoinDateDistributionVM
{
    public string Period { get; set; } = string.Empty; // This Month, Last Month, etc.
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}




