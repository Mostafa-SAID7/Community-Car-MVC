using System.ComponentModel.DataAnnotations;
using CommunityCar.Application.Common.Models.Identity;

namespace CommunityCar.Application.Common.Models.Account;

#region Account Management Requests

public class DeactivateAccountRequest
{
    public Guid UserId { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    public string? Reason { get; set; }
}

public class DeleteAccountRequest
{
    public Guid UserId { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    public string? Reason { get; set; }
}

public class ExportUserDataRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
    public List<string> DataTypes { get; set; } = new();
    public string Format { get; set; } = "JSON";
    public bool IncludeDeleted { get; set; }
}

#endregion

#region Account View Models

public class AccountInfoVM : UserSummaryVM
{
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsDeactivated { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public string? DeactivationReason { get; set; }
    public int ActiveSessions { get; set; }
    public bool HasLinkedAccounts { get; set; }
    public new DateTime CreatedAt { get; set; }
}

public class DataExportVM
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string? DownloadUrl { get; set; }
    public long FileSize { get; set; }
    public string Format { get; set; } = "JSON";
    public List<string> DataTypes { get; set; } = new();
}

public class ConsentVM
{
    public string Type { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public DateTime AcceptedAt { get; set; }
    public string Version { get; set; } = "1.0";
}

#endregion