using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for LeaveRequest entity
/// </summary>
public interface ILeaveRequestRepository
{
    Task<IEnumerable<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LeaveRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);
    Task UpdateAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
}
