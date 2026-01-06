using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Infrastructure.Data;

namespace PotatoDistroHR.Infrastructure.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PayrollRepository> _logger;

    public PayrollRepository(ApplicationDbContext context, ILogger<PayrollRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Payroll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Include(p => p.Employee)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.PayPeriodEnd)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payroll?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<Payroll> AddAsync(Payroll payroll, CancellationToken cancellationToken = default)
    {
        _context.Payrolls.Add(payroll);
        await _context.SaveChangesAsync(cancellationToken);
        return payroll;
    }

    public async Task UpdateAsync(Payroll payroll, CancellationToken cancellationToken = default)
    {
        _context.Payrolls.Update(payroll);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var payroll = await _context.Payrolls.FindAsync(new object[] { id }, cancellationToken);
        if (payroll != null)
        {
            payroll.IsActive = false;
            payroll.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls.AnyAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Payroll>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Include(p => p.Employee)
            .Where(p => p.EmployeeId == employeeId && p.IsActive)
            .OrderByDescending(p => p.PayPeriodEnd)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payroll>> GetByPayPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Include(p => p.Employee)
            .Where(p => p.PayPeriodStart >= startDate && p.PayPeriodEnd <= endDate && p.IsActive)
            .OrderBy(p => p.Employee.LastName)
            .ThenBy(p => p.Employee.FirstName)
            .ToListAsync(cancellationToken);
    }
}
