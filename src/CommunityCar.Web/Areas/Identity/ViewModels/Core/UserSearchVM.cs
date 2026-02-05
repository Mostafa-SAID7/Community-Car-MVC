namespace CommunityCar.Web.Areas.Identity.ViewModels.Core;

public class UserSearchVM
{
    public string SearchTerm { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool ActiveOnly { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public List<UserCardVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasMore { get; set; }
}
