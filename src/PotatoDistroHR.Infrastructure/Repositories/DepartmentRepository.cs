using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Infrastructure.Data;

namespace PotatoDistroHR.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(ApplicationDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Include(d => d.Manager)
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Include(d => d.Manager)
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive, cancellationToken);
    }

    public async Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);
        return department;
    }

    public async Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments.FindAsync(new object[] { id }, cancellationToken);
        if (department != null)
        {
            department.IsActive = false;
            department.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AnyAsync(d => d.Id == id && d.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Department>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Include(d => d.Manager)
            .Where(d => d.IsActive && (d.Name.Contains(searchTerm) || d.Hotline.Contains(searchTerm)))
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }
}
