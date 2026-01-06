using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Exceptions;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Domain.Interfaces.Services;

namespace PotatoDistroHR.Application.Services;

/// <summary>
/// Service implementation for LeaveRequest operations
/// </summary>
public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly ILogger<LeaveRequestService> _logger;

    public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, ILogger<LeaveRequestService> logger)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all leave requests");
            return await _leaveRequestRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all leave requests");
            throw;
        }
    }

    public async Task<LeaveRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving leave request with ID: {LeaveRequestId}", id);
            return await _leaveRequestRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leave request with ID: {LeaveRequestId}", id);
            throw;
        }
    }

    public async Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new leave request for employee ID: {EmployeeId}", leaveRequest.EmployeeId);
            leaveRequest.CreatedDate = DateTime.UtcNow;
            leaveRequest.IsActive = true;
            leaveRequest.Status = "Pending";
            var result = await _leaveRequestRepository.AddAsync(leaveRequest, cancellationToken);
            _logger.LogInformation("Leave request created successfully with ID: {LeaveRequestId}", result.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating leave request for employee ID: {EmployeeId}", leaveRequest.EmployeeId);
            throw;
        }
    }

    public async Task UpdateAsync(int id, LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating leave request with ID: {LeaveRequestId}", id);
            var existingLeaveRequest = await _leaveRequestRepository.GetByIdAsync(id, cancellationToken);
            if (existingLeaveRequest == null)
            {
                throw new EntityNotFoundException(nameof(LeaveRequest), id);
            }

            leaveRequest.Id = id;
            leaveRequest.ModifiedDate = DateTime.UtcNow;
            await _leaveRequestRepository.UpdateAsync(leaveRequest, cancellationToken);
            _logger.LogInformation("Leave request with ID {LeaveRequestId} updated successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating leave request with ID: {LeaveRequestId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting leave request with ID: {LeaveRequestId}", id);
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id, cancellationToken);
            if (leaveRequest == null)
            {
                throw new EntityNotFoundException(nameof(LeaveRequest), id);
            }

            await _leaveRequestRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Leave request with ID {LeaveRequestId} deleted successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting leave request with ID: {LeaveRequestId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving leave requests for employee ID: {EmployeeId}", employeeId);
            return await _leaveRequestRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leave requests for employee ID: {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task ApproveAsync(int id, int approvedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving leave request ID: {LeaveRequestId} by user ID: {ApprovedBy}", id, approvedBy);
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id, cancellationToken);
            if (leaveRequest == null)
            {
                throw new EntityNotFoundException(nameof(LeaveRequest), id);
            }

            leaveRequest.Status = "Approved";
            leaveRequest.ApprovedBy = approvedBy;
            leaveRequest.ApprovedDate = DateTime.UtcNow;
            leaveRequest.ModifiedDate = DateTime.UtcNow;

            await _leaveRequestRepository.UpdateAsync(leaveRequest, cancellationToken);
            _logger.LogInformation("Leave request ID {LeaveRequestId} approved successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving leave request ID: {LeaveRequestId}", id);
            throw;
        }
    }

    public async Task RejectAsync(int id, int rejectedBy, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rejecting leave request ID: {LeaveRequestId} by user ID: {RejectedBy}", id, rejectedBy);
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id, cancellationToken);
            if (leaveRequest == null)
            {
                throw new EntityNotFoundException(nameof(LeaveRequest), id);
            }

            leaveRequest.Status = "Rejected";
            leaveRequest.ApprovedBy = rejectedBy;
            leaveRequest.RejectionReason = reason;
            leaveRequest.ModifiedDate = DateTime.UtcNow;

            await _leaveRequestRepository.UpdateAsync(leaveRequest, cancellationToken);
            _logger.LogInformation("Leave request ID {LeaveRequestId} rejected successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting leave request ID: {LeaveRequestId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all pending leave requests");
            return await _leaveRequestRepository.GetPendingRequestsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending leave requests");
            throw;
        }
    }
}
