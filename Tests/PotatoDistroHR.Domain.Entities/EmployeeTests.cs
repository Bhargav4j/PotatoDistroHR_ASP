using Xunit;
using System;
using System.Collections.Generic;
using PotatoDistroHR.Domain.Entities;

namespace Tests.PotatoDistroHR.Domain.Entities
{
    /// <summary>
    /// Unit tests for Employee entity
    /// </summary>
    public class EmployeeTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var employee = new Employee();

            // Assert
            Assert.Equal(0, employee.Id);
            Assert.Equal(string.Empty, employee.FirstName);
            Assert.Equal(string.Empty, employee.LastName);
            Assert.Equal(string.Empty, employee.Address);
            Assert.Equal(string.Empty, employee.Contact);
            Assert.True(employee.IsActive);
            Assert.Equal(string.Empty, employee.CreatedBy);
            Assert.NotNull(employee.Subordinates);
            Assert.NotNull(employee.LeaveRequests);
            Assert.NotNull(employee.Payrolls);
        }

        [Fact]
        public void Id_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            int expectedId = 100;

            // Act
            employee.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, employee.Id);
        }

        [Fact]
        public void FirstName_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            string expectedName = "John";

            // Act
            employee.FirstName = expectedName;

            // Assert
            Assert.Equal(expectedName, employee.FirstName);
        }

        [Fact]
        public void LastName_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            string expectedName = "Doe";

            // Act
            employee.LastName = expectedName;

            // Assert
            Assert.Equal(expectedName, employee.LastName);
        }

        [Fact]
        public void BirthDate_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            var expectedDate = new DateTime(1990, 5, 15);

            // Act
            employee.BirthDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, employee.BirthDate);
        }

        [Fact]
        public void Address_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            string expectedAddress = "123 Main St";

            // Act
            employee.Address = expectedAddress;

            // Assert
            Assert.Equal(expectedAddress, employee.Address);
        }

        [Fact]
        public void Gender_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            char expectedGender = 'M';

            // Act
            employee.Gender = expectedGender;

            // Assert
            Assert.Equal(expectedGender, employee.Gender);
        }

        [Fact]
        public void Contact_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            string expectedContact = "555-1234";

            // Act
            employee.Contact = expectedContact;

            // Assert
            Assert.Equal(expectedContact, employee.Contact);
        }

        [Fact]
        public void SupervisorId_ShouldSetAndGet_WithValue()
        {
            // Arrange
            var employee = new Employee();
            int? expectedId = 50;

            // Act
            employee.SupervisorId = expectedId;

            // Assert
            Assert.Equal(expectedId, employee.SupervisorId);
        }

        [Fact]
        public void SupervisorId_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var employee = new Employee();

            // Assert
            Assert.Null(employee.SupervisorId);
        }

        [Fact]
        public void StartDate_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            var expectedDate = new DateTime(2020, 1, 1);

            // Act
            employee.StartDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, employee.StartDate);
        }

        [Fact]
        public void EndDate_ShouldSetAndGet_WithValue()
        {
            // Arrange
            var employee = new Employee();
            var expectedDate = new DateTime(2025, 12, 31);

            // Act
            employee.EndDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, employee.EndDate);
        }

        [Fact]
        public void EndDate_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var employee = new Employee();

            // Assert
            Assert.Null(employee.EndDate);
        }

        [Fact]
        public void Salary_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            decimal expectedSalary = 75000.50m;

            // Act
            employee.Salary = expectedSalary;

            // Assert
            Assert.Equal(expectedSalary, employee.Salary);
        }

        [Fact]
        public void DepartmentId_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            int expectedDeptId = 10;

            // Act
            employee.DepartmentId = expectedDeptId;

            // Assert
            Assert.Equal(expectedDeptId, employee.DepartmentId);
        }

        [Fact]
        public void IsActive_ShouldDefaultToTrue()
        {
            // Arrange & Act
            var employee = new Employee();

            // Assert
            Assert.True(employee.IsActive);
        }

        [Fact]
        public void IsActive_ShouldSetToFalse()
        {
            // Arrange
            var employee = new Employee();

            // Act
            employee.IsActive = false;

            // Assert
            Assert.False(employee.IsActive);
        }

        [Fact]
        public void ModifiedDate_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            var expectedDate = DateTime.UtcNow;

            // Act
            employee.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, employee.ModifiedDate);
        }

        [Fact]
        public void ModifiedBy_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            string expectedUser = "admin";

            // Act
            employee.ModifiedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, employee.ModifiedBy);
        }

        [Fact]
        public void Department_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            var department = new Department { Id = 1, Name = "IT" };

            // Act
            employee.Department = department;

            // Assert
            Assert.NotNull(employee.Department);
            Assert.Equal(1, employee.Department.Id);
        }

        [Fact]
        public void Supervisor_ShouldSetAndGet()
        {
            // Arrange
            var employee = new Employee();
            var supervisor = new Employee { Id = 99, FirstName = "Manager" };

            // Act
            employee.Supervisor = supervisor;

            // Assert
            Assert.NotNull(employee.Supervisor);
            Assert.Equal(99, employee.Supervisor.Id);
        }

        [Fact]
        public void Subordinates_ShouldInitializeAsEmptyCollection()
        {
            // Arrange & Act
            var employee = new Employee();

            // Assert
            Assert.NotNull(employee.Subordinates);
            Assert.Empty(employee.Subordinates);
        }

        [Fact]
        public void LeaveRequests_ShouldInitializeAsEmptyCollection()
        {
            // Arrange & Act
            var employee = new Employee();

            // Assert
            Assert.NotNull(employee.LeaveRequests);
            Assert.Empty(employee.LeaveRequests);
        }

        [Fact]
        public void Payrolls_ShouldInitializeAsEmptyCollection()
        {
            // Arrange & Act
            var employee = new Employee();

            // Assert
            Assert.NotNull(employee.Payrolls);
            Assert.Empty(employee.Payrolls);
        }

        [Fact]
        public void Employee_ShouldSupportFullPropertyAssignment()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                BirthDate = new DateTime(1985, 3, 20),
                Address = "456 Oak Ave",
                Gender = 'F',
                Contact = "555-9876",
                SupervisorId = 10,
                StartDate = new DateTime(2015, 6, 1),
                EndDate = null,
                Salary = 95000m,
                DepartmentId = 5,
                IsActive = true,
                CreatedBy = "system"
            };

            // Assert
            Assert.Equal(1, employee.Id);
            Assert.Equal("Jane", employee.FirstName);
            Assert.Equal("Smith", employee.LastName);
            Assert.Equal(new DateTime(1985, 3, 20), employee.BirthDate);
            Assert.Equal("456 Oak Ave", employee.Address);
            Assert.Equal('F', employee.Gender);
            Assert.Equal("555-9876", employee.Contact);
            Assert.Equal(10, employee.SupervisorId);
            Assert.Equal(new DateTime(2015, 6, 1), employee.StartDate);
            Assert.Null(employee.EndDate);
            Assert.Equal(95000m, employee.Salary);
            Assert.Equal(5, employee.DepartmentId);
            Assert.True(employee.IsActive);
            Assert.Equal("system", employee.CreatedBy);
        }
    }
}
