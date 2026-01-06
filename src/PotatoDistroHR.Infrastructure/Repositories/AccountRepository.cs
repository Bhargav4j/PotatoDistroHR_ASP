using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Infrastructure.Data;

namespace PotatoDistroHR.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(ApplicationDbContext context, ILogger<AccountRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Include(a => a.Employee)
            .Where(a => a.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == id && a.IsActive, cancellationToken);
    }

    public async Task<Account?> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.IsActive, cancellationToken);
    }

    public async Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);
        return account;
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var account = await _context.Accounts.FindAsync(new object[] { id }, cancellationToken);
        if (account != null)
        {
            account.IsActive = false;
            account.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts.AnyAsync(a => a.Id == id && a.IsActive, cancellationToken);
    }

    public async Task<Account?> ValidateCredentialsAsync(int employeeId, string passwordHash, bool isAdmin, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId &&
                a.PasswordHash == passwordHash &&
                a.IsAdmin == isAdmin &&
                a.IsActive,
                cancellationToken);
    }
}
