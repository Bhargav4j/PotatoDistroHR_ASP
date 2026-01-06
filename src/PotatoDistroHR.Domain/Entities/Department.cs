namespace PotatoDistroHR.Domain.Entities;

/// <summary>
/// Represents a department in the organization
/// </summary>
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Hotline { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public Employee? Manager { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
