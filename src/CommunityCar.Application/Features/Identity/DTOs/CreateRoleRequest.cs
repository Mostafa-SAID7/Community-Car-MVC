using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Identity.DTOs;

/// <summary>
/// Request model for creating a new role
/// </summary>
public class CreateRoleRequest
{
    [Required]
    [StringLength(256, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}


