using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Employee entity
/// </summary>
public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);
    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<Employee?> GetBySupervisorIdAsync(int supervisorId, CancellationToken cancellationToken = default);
}
