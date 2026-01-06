using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Infrastructure.Data;

namespace PotatoDistroHR.Infrastructure.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LeaveRequestRepository> _logger;

    public LeaveRequestRepository(ApplicationDbContext context, ILogger<LeaveRequestRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .Include(l => l.Employee)
            .Include(l => l.Approver)
            .Where(l => l.IsActive)
            .OrderByDescending(l => l.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<LeaveRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .Include(l => l.Employee)
            .Include(l => l.Approver)
            .FirstOrDefaultAsync(l => l.Id == id && l.IsActive, cancellationToken);
    }

    public async Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync(cancellationToken);
        return leaveRequest;
    }

    public async Task UpdateAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        _context.LeaveRequests.Update(leaveRequest);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(new object[] { id }, cancellationToken);
        if (leaveRequest != null)
        {
            leaveRequest.IsActive = false;
            leaveRequest.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests.AnyAsync(l => l.Id == id && l.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .Include(l => l.Employee)
            .Include(l => l.Approver)
            .Where(l => l.EmployeeId == employeeId && l.IsActive)
            .OrderByDescending(l => l.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .Include(l => l.Employee)
            .Where(l => l.Status == "Pending" && l.IsActive)
            .OrderBy(l => l.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
