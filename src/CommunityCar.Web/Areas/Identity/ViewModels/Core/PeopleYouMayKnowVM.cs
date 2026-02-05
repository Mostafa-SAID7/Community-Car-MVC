namespace CommunityCar.Web.Areas.Identity.ViewModels.Core;

public class PeopleYouMayKnowVM
{
    public List<UserSuggestionVM> Suggestions { get; set; } = new();
    public int TotalSuggestions { get; set; }
    public bool HasMore { get; set; }
}
