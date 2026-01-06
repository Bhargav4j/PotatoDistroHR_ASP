using Xunit;
using System;
using PotatoDistroHR.Domain.Entities;

namespace Tests.PotatoDistroHR.Domain.Entities
{
    /// <summary>
    /// Unit tests for Account entity
    /// </summary>
    public class AccountTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var account = new Account();

            // Assert
            Assert.Equal(0, account.Id);
            Assert.Equal(0, account.EmployeeId);
            Assert.Equal(string.Empty, account.PasswordHash);
            Assert.False(account.IsAdmin);
            Assert.True(account.IsActive);
            Assert.Equal(string.Empty, account.CreatedBy);
        }

        [Fact]
        public void Id_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            int expectedId = 42;

            // Act
            account.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, account.Id);
        }

        [Fact]
        public void EmployeeId_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            int expectedEmployeeId = 100;

            // Act
            account.EmployeeId = expectedEmployeeId;

            // Assert
            Assert.Equal(expectedEmployeeId, account.EmployeeId);
        }

        [Fact]
        public void PasswordHash_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            string expectedHash = "$2a$11$abcd1234efgh5678ijkl9012mnop3456qrst7890uvwx1234yz";

            // Act
            account.PasswordHash = expectedHash;

            // Assert
            Assert.Equal(expectedHash, account.PasswordHash);
        }

        [Fact]
        public void IsAdmin_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var account = new Account();

            // Assert
            Assert.False(account.IsAdmin);
        }

        [Fact]
        public void IsAdmin_ShouldSetToTrue()
        {
            // Arrange
            var account = new Account();

            // Act
            account.IsAdmin = true;

            // Assert
            Assert.True(account.IsAdmin);
        }

        [Fact]
        public void IsActive_ShouldDefaultToTrue()
        {
            // Arrange & Act
            var account = new Account();

            // Assert
            Assert.True(account.IsActive);
        }

        [Fact]
        public void IsActive_ShouldSetToFalse()
        {
            // Arrange
            var account = new Account();

            // Act
            account.IsActive = false;

            // Assert
            Assert.False(account.IsActive);
        }

        [Fact]
        public void CreatedDate_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            var expectedDate = DateTime.UtcNow;

            // Act
            account.CreatedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, account.CreatedDate);
        }

        [Fact]
        public void ModifiedDate_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            var expectedDate = DateTime.UtcNow;

            // Act
            account.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, account.ModifiedDate);
        }

        [Fact]
        public void ModifiedDate_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var account = new Account();

            // Assert
            Assert.Null(account.ModifiedDate);
        }

        [Fact]
        public void LastLoginDate_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            var expectedDate = DateTime.UtcNow;

            // Act
            account.LastLoginDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, account.LastLoginDate);
        }

        [Fact]
        public void LastLoginDate_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var account = new Account();

            // Assert
            Assert.Null(account.LastLoginDate);
        }

        [Fact]
        public void CreatedBy_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            string expectedUser = "admin";

            // Act
            account.CreatedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, account.CreatedBy);
        }

        [Fact]
        public void ModifiedBy_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            string expectedUser = "admin";

            // Act
            account.ModifiedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, account.ModifiedBy);
        }

        [Fact]
        public void ModifiedBy_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var account = new Account();

            // Assert
            Assert.Null(account.ModifiedBy);
        }

        [Fact]
        public void Employee_ShouldSetAndGet()
        {
            // Arrange
            var account = new Account();
            var employee = new Employee { Id = 1, FirstName = "John", LastName = "Doe" };

            // Act
            account.Employee = employee;

            // Assert
            Assert.NotNull(account.Employee);
            Assert.Equal(1, account.Employee.Id);
            Assert.Equal("John", account.Employee.FirstName);
        }

        [Fact]
        public void Account_ShouldSupportFullPropertyAssignment()
        {
            // Arrange
            var account = new Account
            {
                Id = 1,
                EmployeeId = 50,
                PasswordHash = "$2a$11$hashedpassword",
                IsAdmin = true,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow.AddDays(-1),
                CreatedBy = "system"
            };

            // Assert
            Assert.Equal(1, account.Id);
            Assert.Equal(50, account.EmployeeId);
            Assert.Equal("$2a$11$hashedpassword", account.PasswordHash);
            Assert.True(account.IsAdmin);
            Assert.True(account.IsActive);
            Assert.Equal("system", account.CreatedBy);
            Assert.NotNull(account.LastLoginDate);
        }

        [Fact]
        public void PasswordHash_ShouldAcceptEmptyString()
        {
            // Arrange
            var account = new Account();

            // Act
            account.PasswordHash = "";

            // Assert
            Assert.Equal(string.Empty, account.PasswordHash);
        }
    }
}
