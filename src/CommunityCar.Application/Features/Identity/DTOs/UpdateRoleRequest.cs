using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Identity.DTOs;

/// <summary>
/// Request model for updating an existing role
/// </summary>
public class UpdateRoleRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(256, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}


