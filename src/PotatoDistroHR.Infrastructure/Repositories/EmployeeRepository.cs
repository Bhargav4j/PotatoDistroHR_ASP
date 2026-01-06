using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Infrastructure.Data;

namespace PotatoDistroHR.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeeRepository> _logger;

    public EmployeeRepository(ApplicationDbContext context, ILogger<EmployeeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Supervisor)
            .Where(e => e.IsActive)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Supervisor)
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive, cancellationToken);
    }

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees.FindAsync(new object[] { id }, cancellationToken);
        if (employee != null)
        {
            employee.IsActive = false;
            employee.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id && e.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => e.IsActive &&
                (e.FirstName.Contains(searchTerm) || e.LastName.Contains(searchTerm) || e.Contact.Contains(searchTerm)))
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => e.DepartmentId == departmentId && e.IsActive)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetBySupervisorIdAsync(int supervisorId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.SupervisorId == supervisorId && e.IsActive, cancellationToken);
    }
}
