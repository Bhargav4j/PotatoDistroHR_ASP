namespace PotatoDistroHR.Domain.Entities;

/// <summary>
/// Represents an employee in the HR system
/// </summary>
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Contact { get; set; } = string.Empty;
    public int? SupervisorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public Department Department { get; set; } = null!;
    public Employee? Supervisor { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    public Account? Account { get; set; }
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
}
