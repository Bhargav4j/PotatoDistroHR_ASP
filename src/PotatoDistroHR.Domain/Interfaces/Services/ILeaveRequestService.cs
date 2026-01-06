using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Domain.Interfaces.Services;

/// <summary>
/// Service interface for LeaveRequest operations
/// </summary>
public interface ILeaveRequestService
{
    Task<IEnumerable<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LeaveRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, LeaveRequest leaveRequest, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task ApproveAsync(int id, int approvedBy, CancellationToken cancellationToken = default);
    Task RejectAsync(int id, int rejectedBy, string reason, CancellationToken cancellationToken = default);
    Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
}
