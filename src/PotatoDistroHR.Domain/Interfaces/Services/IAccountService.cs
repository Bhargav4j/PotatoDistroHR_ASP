using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Account operations
/// </summary>
public interface IAccountService
{
    Task<Account?> ValidateCredentialsAsync(int employeeId, string password, bool isAdmin, CancellationToken cancellationToken = default);
    Task<Account> CreateAsync(int employeeId, string password, bool isAdmin, CancellationToken cancellationToken = default);
    Task UpdatePasswordAsync(int employeeId, string newPassword, CancellationToken cancellationToken = default);
    Task<Account?> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
}
