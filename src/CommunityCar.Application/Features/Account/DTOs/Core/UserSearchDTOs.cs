namespace CommunityCar.Application.Features.Account.DTOs.Core;

#region User Search DTOs

public class UserSearchRequest
{
    public string SearchTerm { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool ActiveOnly { get; set; } = true;
    public string? SortBy { get; set; } = "FullName";
    public bool SortDescending { get; set; } = false;
}

public class UserSearchResult
{
    public List<UserDTO> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasMore { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
}

public class UserFilterRequest
{
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsEmailConfirmed { get; set; }
    public bool? IsTwoFactorEnabled { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? LastLoginAfter { get; set; }
    public DateTime? LastLoginBefore { get; set; }
}

#endregion