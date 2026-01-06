using Xunit;
using System;
using PotatoDistroHR.Domain.Entities;

namespace Tests.PotatoDistroHR.Domain.Entities
{
    /// <summary>
    /// Unit tests for LeaveRequest entity
    /// </summary>
    public class LeaveRequestTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var leaveRequest = new LeaveRequest();

            // Assert
            Assert.Equal(0, leaveRequest.Id);
            Assert.Equal(0, leaveRequest.EmployeeId);
            Assert.Equal(string.Empty, leaveRequest.Reason);
            Assert.Equal("Pending", leaveRequest.Status);
            Assert.True(leaveRequest.IsActive);
            Assert.Equal(string.Empty, leaveRequest.CreatedBy);
        }

        [Fact]
        public void Id_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            int expectedId = 25;

            // Act
            leaveRequest.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, leaveRequest.Id);
        }

        [Fact]
        public void EmployeeId_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            int expectedEmployeeId = 100;

            // Act
            leaveRequest.EmployeeId = expectedEmployeeId;

            // Assert
            Assert.Equal(expectedEmployeeId, leaveRequest.EmployeeId);
        }

        [Fact]
        public void StartDate_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            var expectedDate = new DateTime(2026, 2, 1);

            // Act
            leaveRequest.StartDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, leaveRequest.StartDate);
        }

        [Fact]
        public void EndDate_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            var expectedDate = new DateTime(2026, 2, 10);

            // Act
            leaveRequest.EndDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, leaveRequest.EndDate);
        }

        [Fact]
        public void Reason_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            string expectedReason = "Family vacation";

            // Act
            leaveRequest.Reason = expectedReason;

            // Assert
            Assert.Equal(expectedReason, leaveRequest.Reason);
        }

        [Fact]
        public void Status_ShouldDefaultToPending()
        {
            // Arrange & Act
            var leaveRequest = new LeaveRequest();

            // Assert
            Assert.Equal("Pending", leaveRequest.Status);
        }

        [Fact]
        public void Status_ShouldSetToApproved()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();

            // Act
            leaveRequest.Status = "Approved";

            // Assert
            Assert.Equal("Approved", leaveRequest.Status);
        }

        [Fact]
        public void Status_ShouldSetToRejected()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();

            // Act
            leaveRequest.Status = "Rejected";

            // Assert
            Assert.Equal("Rejected", leaveRequest.Status);
        }

        [Fact]
        public void ApprovedBy_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            int? expectedApproverId = 50;

            // Act
            leaveRequest.ApprovedBy = expectedApproverId;

            // Assert
            Assert.Equal(expectedApproverId, leaveRequest.ApprovedBy);
        }

        [Fact]
        public void ApprovedBy_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var leaveRequest = new LeaveRequest();

            // Assert
            Assert.Null(leaveRequest.ApprovedBy);
        }

        [Fact]
        public void ApprovedDate_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            var expectedDate = DateTime.UtcNow;

            // Act
            leaveRequest.ApprovedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, leaveRequest.ApprovedDate);
        }

        [Fact]
        public void ApprovedDate_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var leaveRequest = new LeaveRequest();

            // Assert
            Assert.Null(leaveRequest.ApprovedDate);
        }

        [Fact]
        public void RejectionReason_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            string expectedReason = "Insufficient leave balance";

            // Act
            leaveRequest.RejectionReason = expectedReason;

            // Assert
            Assert.Equal(expectedReason, leaveRequest.RejectionReason);
        }

        [Fact]
        public void RejectionReason_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var leaveRequest = new LeaveRequest();

            // Assert
            Assert.Null(leaveRequest.RejectionReason);
        }

        [Fact]
        public void IsActive_ShouldDefaultToTrue()
        {
            // Arrange & Act
            var leaveRequest = new LeaveRequest();

            // Assert
            Assert.True(leaveRequest.IsActive);
        }

        [Fact]
        public void IsActive_ShouldSetToFalse()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();

            // Act
            leaveRequest.IsActive = false;

            // Assert
            Assert.False(leaveRequest.IsActive);
        }

        [Fact]
        public void CreatedDate_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            var expectedDate = DateTime.UtcNow;

            // Act
            leaveRequest.CreatedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, leaveRequest.CreatedDate);
        }

        [Fact]
        public void ModifiedDate_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            var expectedDate = DateTime.UtcNow;

            // Act
            leaveRequest.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, leaveRequest.ModifiedDate);
        }

        [Fact]
        public void ModifiedDate_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var leaveRequest = new LeaveRequest();

            // Assert
            Assert.Null(leaveRequest.ModifiedDate);
        }

        [Fact]
        public void CreatedBy_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            string expectedUser = "employee1";

            // Act
            leaveRequest.CreatedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, leaveRequest.CreatedBy);
        }

        [Fact]
        public void ModifiedBy_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            string expectedUser = "admin";

            // Act
            leaveRequest.ModifiedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, leaveRequest.ModifiedBy);
        }

        [Fact]
        public void ModifiedBy_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var leaveRequest = new LeaveRequest();

            // Assert
            Assert.Null(leaveRequest.ModifiedBy);
        }

        [Fact]
        public void Employee_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            var employee = new Employee { Id = 1, FirstName = "John", LastName = "Doe" };

            // Act
            leaveRequest.Employee = employee;

            // Assert
            Assert.NotNull(leaveRequest.Employee);
            Assert.Equal(1, leaveRequest.Employee.Id);
        }

        [Fact]
        public void Approver_ShouldSetAndGet()
        {
            // Arrange
            var leaveRequest = new LeaveRequest();
            var approver = new Employee { Id = 2, FirstName = "Manager", LastName = "Smith" };

            // Act
            leaveRequest.Approver = approver;

            // Assert
            Assert.NotNull(leaveRequest.Approver);
            Assert.Equal(2, leaveRequest.Approver.Id);
        }

        [Fact]
        public void LeaveRequest_ShouldSupportFullPropertyAssignment()
        {
            // Arrange
            var leaveRequest = new LeaveRequest
            {
                Id = 1,
                EmployeeId = 10,
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 3, 5),
                Reason = "Medical leave",
                Status = "Approved",
                ApprovedBy = 5,
                ApprovedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "employee1"
            };

            // Assert
            Assert.Equal(1, leaveRequest.Id);
            Assert.Equal(10, leaveRequest.EmployeeId);
            Assert.Equal(new DateTime(2026, 3, 1), leaveRequest.StartDate);
            Assert.Equal(new DateTime(2026, 3, 5), leaveRequest.EndDate);
            Assert.Equal("Medical leave", leaveRequest.Reason);
            Assert.Equal("Approved", leaveRequest.Status);
            Assert.Equal(5, leaveRequest.ApprovedBy);
            Assert.NotNull(leaveRequest.ApprovedDate);
            Assert.True(leaveRequest.IsActive);
            Assert.Equal("employee1", leaveRequest.CreatedBy);
        }
    }
}
