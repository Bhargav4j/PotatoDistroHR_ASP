using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Department operations
/// </summary>
public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Department> CreateAsync(Department department, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, Department department, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
