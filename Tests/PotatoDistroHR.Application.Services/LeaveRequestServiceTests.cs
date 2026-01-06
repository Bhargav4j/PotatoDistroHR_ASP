using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PotatoDistroHR.Application.Services;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Exceptions;
using PotatoDistroHR.Domain.Interfaces.Repositories;

namespace Tests.PotatoDistroHR.Application.Services
{
    /// <summary>
    /// Unit tests for LeaveRequestService
    /// </summary>
    public class LeaveRequestServiceTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockRepository;
        private readonly Mock<ILogger<LeaveRequestService>> _mockLogger;
        private readonly LeaveRequestService _service;

        public LeaveRequestServiceTests()
        {
            _mockRepository = new Mock<ILeaveRequestRepository>();
            _mockLogger = new Mock<ILogger<LeaveRequestService>>();
            _service = new LeaveRequestService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllLeaveRequests()
        {
            // Arrange
            var leaveRequests = new List<LeaveRequest>
            {
                new LeaveRequest { Id = 1, EmployeeId = 10, Status = "Pending" },
                new LeaveRequest { Id = 2, EmployeeId = 20, Status = "Approved" }
            };
            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(leaveRequests);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<LeaveRequest>)result).Count);
            _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnLeaveRequest()
        {
            // Arrange
            var leaveRequest = new LeaveRequest { Id = 1, EmployeeId = 10 };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(leaveRequest);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateLeaveRequest()
        {
            // Arrange
            var leaveRequest = new LeaveRequest { EmployeeId = 10, Reason = "Vacation" };
            var createdLeaveRequest = new LeaveRequest { Id = 1, EmployeeId = 10, Status = "Pending" };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdLeaveRequest);

            // Act
            var result = await _service.CreateAsync(leaveRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Pending", result.Status);
            Assert.True(result.IsActive);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldUpdateLeaveRequest()
        {
            // Arrange
            var existingLeaveRequest = new LeaveRequest { Id = 1, EmployeeId = 10 };
            var updatedLeaveRequest = new LeaveRequest { Id = 1, EmployeeId = 10, Reason = "Updated reason" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingLeaveRequest);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(1, updatedLeaveRequest);

            // Assert
            _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var leaveRequest = new LeaveRequest { Id = 999 };
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LeaveRequest?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(999, leaveRequest));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteLeaveRequest()
        {
            // Arrange
            var leaveRequest = new LeaveRequest { Id = 1, EmployeeId = 10 };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(leaveRequest);
            _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(1);

            // Assert
            _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LeaveRequest?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(999));
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_ShouldReturnEmployeeLeaveRequests()
        {
            // Arrange
            var leaveRequests = new List<LeaveRequest>
            {
                new LeaveRequest { Id = 1, EmployeeId = 10 },
                new LeaveRequest { Id = 2, EmployeeId = 10 }
            };
            _mockRepository.Setup(r => r.GetByEmployeeIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(leaveRequests);

            // Act
            var result = await _service.GetByEmployeeIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<LeaveRequest>)result).Count);
            _mockRepository.Verify(r => r.GetByEmployeeIdAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_WithValidId_ShouldApproveLeaveRequest()
        {
            // Arrange
            var leaveRequest = new LeaveRequest { Id = 1, EmployeeId = 10, Status = "Pending" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(leaveRequest);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.ApproveAsync(1, 5);

            // Assert
            Assert.Equal("Approved", leaveRequest.Status);
            Assert.Equal(5, leaveRequest.ApprovedBy);
            Assert.NotNull(leaveRequest.ApprovedDate);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LeaveRequest?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.ApproveAsync(999, 5));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RejectAsync_WithValidId_ShouldRejectLeaveRequest()
        {
            // Arrange
            var leaveRequest = new LeaveRequest { Id = 1, EmployeeId = 10, Status = "Pending" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(leaveRequest);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.RejectAsync(1, 5, "Insufficient leave balance");

            // Assert
            Assert.Equal("Rejected", leaveRequest.Status);
            Assert.Equal(5, leaveRequest.ApprovedBy);
            Assert.Equal("Insufficient leave balance", leaveRequest.RejectionReason);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RejectAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LeaveRequest?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.RejectAsync(999, 5, "reason"));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetPendingRequestsAsync_ShouldReturnPendingRequests()
        {
            // Arrange
            var pendingRequests = new List<LeaveRequest>
            {
                new LeaveRequest { Id = 1, Status = "Pending" },
                new LeaveRequest { Id = 2, Status = "Pending" }
            };
            _mockRepository.Setup(r => r.GetPendingRequestsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(pendingRequests);

            // Act
            var result = await _service.GetPendingRequestsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<LeaveRequest>)result).Count);
            _mockRepository.Verify(r => r.GetPendingRequestsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldSetDefaultStatus()
        {
            // Arrange
            var leaveRequest = new LeaveRequest { EmployeeId = 10 };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((LeaveRequest lr, CancellationToken ct) => lr);

            // Act
            var result = await _service.CreateAsync(leaveRequest);

            // Assert
            Assert.Equal("Pending", result.Status);
            Assert.True(result.IsActive);
        }
    }
}
