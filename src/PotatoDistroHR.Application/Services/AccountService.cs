using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Exceptions;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Domain.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace PotatoDistroHR.Application.Services;

/// <summary>
/// Service implementation for Account operations
/// </summary>
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<Account?> ValidateCredentialsAsync(int employeeId, string password, bool isAdmin, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating credentials for employee ID: {EmployeeId}", employeeId);
            var passwordHash = HashPassword(password);
            var account = await _accountRepository.ValidateCredentialsAsync(employeeId, passwordHash, isAdmin, cancellationToken);

            if (account != null)
            {
                account.LastLoginDate = DateTime.UtcNow;
                await _accountRepository.UpdateAsync(account, cancellationToken);
                _logger.LogInformation("Credentials validated successfully for employee ID: {EmployeeId}", employeeId);
            }
            else
            {
                _logger.LogWarning("Invalid credentials for employee ID: {EmployeeId}", employeeId);
            }

            return account;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating credentials for employee ID: {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task<Account> CreateAsync(int employeeId, string password, bool isAdmin, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating account for employee ID: {EmployeeId}", employeeId);
            var passwordHash = HashPassword(password);

            var account = new Account
            {
                EmployeeId = employeeId,
                PasswordHash = passwordHash,
                IsAdmin = isAdmin,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var result = await _accountRepository.AddAsync(account, cancellationToken);
            _logger.LogInformation("Account created successfully for employee ID: {EmployeeId}", employeeId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account for employee ID: {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task UpdatePasswordAsync(int employeeId, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating password for employee ID: {EmployeeId}", employeeId);
            var account = await _accountRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);

            if (account == null)
            {
                throw new EntityNotFoundException($"Account for employee ID {employeeId} not found");
            }

            account.PasswordHash = HashPassword(newPassword);
            account.ModifiedDate = DateTime.UtcNow;
            account.ModifiedBy = "System";

            await _accountRepository.UpdateAsync(account, cancellationToken);
            _logger.LogInformation("Password updated successfully for employee ID: {EmployeeId}", employeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password for employee ID: {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task<Account?> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving account for employee ID: {EmployeeId}", employeeId);
            return await _accountRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account for employee ID: {EmployeeId}", employeeId);
            throw;
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
