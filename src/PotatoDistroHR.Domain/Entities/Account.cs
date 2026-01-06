namespace PotatoDistroHR.Domain.Entities;

/// <summary>
/// Represents user account for authentication
/// </summary>
public class Account
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;
}
