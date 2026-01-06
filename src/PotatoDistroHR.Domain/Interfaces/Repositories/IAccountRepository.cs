using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Account entity
/// </summary>
public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Account?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Account?> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default);
    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<Account?> ValidateCredentialsAsync(int employeeId, string passwordHash, bool isAdmin, CancellationToken cancellationToken = default);
}
