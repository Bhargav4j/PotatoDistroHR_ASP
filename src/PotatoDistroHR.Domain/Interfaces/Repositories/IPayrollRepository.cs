using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Payroll entity
/// </summary>
public interface IPayrollRepository
{
    Task<IEnumerable<Payroll>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Payroll?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Payroll> AddAsync(Payroll payroll, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payroll payroll, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payroll>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payroll>> GetByPayPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
