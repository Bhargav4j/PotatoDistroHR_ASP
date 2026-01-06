using Xunit;
using System;
using System.Collections.Generic;
using PotatoDistroHR.Domain.Entities;

namespace Tests.PotatoDistroHR.Domain.Entities
{
    /// <summary>
    /// Unit tests for Department entity
    /// </summary>
    public class DepartmentTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var department = new Department();

            // Assert
            Assert.Equal(0, department.Id);
            Assert.Equal(string.Empty, department.Name);
            Assert.Equal(string.Empty, department.Hotline);
            Assert.True(department.IsActive);
            Assert.Equal(string.Empty, department.CreatedBy);
            Assert.NotNull(department.Employees);
            Assert.Empty(department.Employees);
        }

        [Fact]
        public void Id_ShouldSetAndGet()
        {
            // Arrange
            var department = new Department();
            int expectedId = 50;

            // Act
            department.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, department.Id);
        }

        [Fact]
        public void Name_ShouldSetAndGet()
        {
            // Arrange
            var department = new Department();
            string expectedName = "Human Resources";

            // Act
            department.Name = expectedName;

            // Assert
            Assert.Equal(expectedName, department.Name);
        }

        [Fact]
        public void Hotline_ShouldSetAndGet()
        {
            // Arrange
            var department = new Department();
            string expectedHotline = "555-HR-HELP";

            // Act
            department.Hotline = expectedHotline;

            // Assert
            Assert.Equal(expectedHotline, department.Hotline);
        }

        [Fact]
        public void ManagerId_ShouldSetAndGet_WithValue()
        {
            // Arrange
            var department = new Department();
            int? expectedId = 100;

            // Act
            department.ManagerId = expectedId;

            // Assert
            Assert.Equal(expectedId, department.ManagerId);
        }

        [Fact]
        public void ManagerId_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var department = new Department();

            // Assert
            Assert.Null(department.ManagerId);
        }

        [Fact]
        public void IsActive_ShouldDefaultToTrue()
        {
            // Arrange & Act
            var department = new Department();

            // Assert
            Assert.True(department.IsActive);
        }

        [Fact]
        public void IsActive_ShouldSetToFalse()
        {
            // Arrange
            var department = new Department();

            // Act
            department.IsActive = false;

            // Assert
            Assert.False(department.IsActive);
        }

        [Fact]
        public void CreatedDate_ShouldBeSet()
        {
            // Arrange
            var department = new Department();
            var expectedDate = DateTime.UtcNow;

            // Act
            department.CreatedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, department.CreatedDate);
        }

        [Fact]
        public void ModifiedDate_ShouldSetAndGet()
        {
            // Arrange
            var department = new Department();
            var expectedDate = DateTime.UtcNow;

            // Act
            department.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, department.ModifiedDate);
        }

        [Fact]
        public void ModifiedDate_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var department = new Department();

            // Assert
            Assert.Null(department.ModifiedDate);
        }

        [Fact]
        public void CreatedBy_ShouldSetAndGet()
        {
            // Arrange
            var department = new Department();
            string expectedUser = "admin";

            // Act
            department.CreatedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, department.CreatedBy);
        }

        [Fact]
        public void ModifiedBy_ShouldSetAndGet()
        {
            // Arrange
            var department = new Department();
            string expectedUser = "admin";

            // Act
            department.ModifiedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, department.ModifiedBy);
        }

        [Fact]
        public void ModifiedBy_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var department = new Department();

            // Assert
            Assert.Null(department.ModifiedBy);
        }

        [Fact]
        public void Manager_ShouldSetAndGet()
        {
            // Arrange
            var department = new Department();
            var manager = new Employee { Id = 1, FirstName = "John", LastName = "Manager" };

            // Act
            department.Manager = manager;

            // Assert
            Assert.NotNull(department.Manager);
            Assert.Equal(1, department.Manager.Id);
            Assert.Equal("John", department.Manager.FirstName);
        }

        [Fact]
        public void Employees_ShouldInitializeAsEmptyCollection()
        {
            // Arrange & Act
            var department = new Department();

            // Assert
            Assert.NotNull(department.Employees);
            Assert.Empty(department.Employees);
        }

        [Fact]
        public void Employees_ShouldAddEmployee()
        {
            // Arrange
            var department = new Department();
            var employee = new Employee { Id = 1, FirstName = "Jane", LastName = "Doe" };

            // Act
            department.Employees.Add(employee);

            // Assert
            Assert.Single(department.Employees);
            Assert.Contains(employee, department.Employees);
        }

        [Fact]
        public void Department_ShouldSupportFullPropertyAssignment()
        {
            // Arrange
            var department = new Department
            {
                Id = 10,
                Name = "Engineering",
                Hotline = "555-ENG-DEPT",
                ManagerId = 25,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system"
            };

            // Assert
            Assert.Equal(10, department.Id);
            Assert.Equal("Engineering", department.Name);
            Assert.Equal("555-ENG-DEPT", department.Hotline);
            Assert.Equal(25, department.ManagerId);
            Assert.True(department.IsActive);
            Assert.Equal("system", department.CreatedBy);
        }

        [Fact]
        public void Name_ShouldAcceptEmptyString()
        {
            // Arrange
            var department = new Department();

            // Act
            department.Name = "";

            // Assert
            Assert.Equal(string.Empty, department.Name);
        }

        [Fact]
        public void Hotline_ShouldAcceptEmptyString()
        {
            // Arrange
            var department = new Department();

            // Act
            department.Hotline = "";

            // Assert
            Assert.Equal(string.Empty, department.Hotline);
        }
    }
}
