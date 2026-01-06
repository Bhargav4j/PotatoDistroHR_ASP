using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Payroll operations
/// </summary>
public interface IPayrollService
{
    Task<IEnumerable<Payroll>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Payroll?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Payroll> CreateAsync(Payroll payroll, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, Payroll payroll, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payroll>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payroll>> GetByPayPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Payroll> GeneratePayrollAsync(int employeeId, DateTime payPeriodStart, DateTime payPeriodEnd, CancellationToken cancellationToken = default);
}
