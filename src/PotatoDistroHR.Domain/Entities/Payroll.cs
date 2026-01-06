namespace PotatoDistroHR.Domain.Entities;

/// <summary>
/// Represents employee payroll information
/// </summary>
public class Payroll
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public DateTime PayDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Processed, Paid
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;
}
