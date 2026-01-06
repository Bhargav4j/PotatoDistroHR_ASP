using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using PotatoDistroHR.Application.Services;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Exceptions;
using PotatoDistroHR.Domain.Interfaces.Repositories;

namespace Tests.PotatoDistroHR.Application.Services
{
    /// <summary>
    /// Unit tests for AccountService
    /// </summary>
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _mockRepository;
        private readonly Mock<ILogger<AccountService>> _mockLogger;
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            _mockRepository = new Mock<IAccountRepository>();
            _mockLogger = new Mock<ILogger<AccountService>>();
            _service = new AccountService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_WithValidCredentials_ShouldReturnAccount()
        {
            // Arrange
            var account = new Account { Id = 1, EmployeeId = 100, IsAdmin = false };
            _mockRepository.Setup(r => r.ValidateCredentialsAsync(100, It.IsAny<string>(), false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.ValidateCredentialsAsync(100, "password123", false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.EmployeeId);
            Assert.NotNull(result.LastLoginDate);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_WithInvalidCredentials_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.ValidateCredentialsAsync(100, It.IsAny<string>(), false, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            // Act
            var result = await _service.ValidateCredentialsAsync(100, "wrongpassword", false);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateAccountWithHashedPassword()
        {
            // Arrange
            var account = new Account { Id = 1, EmployeeId = 100, IsAdmin = false };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var result = await _service.CreateAsync(100, "password123", false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.EmployeeId);
            Assert.False(result.IsAdmin);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithAdminFlag_ShouldCreateAdminAccount()
        {
            // Arrange
            var account = new Account { Id = 1, EmployeeId = 100, IsAdmin = true };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var result = await _service.CreateAsync(100, "adminpass", true);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsAdmin);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePasswordAsync_WithValidEmployeeId_ShouldUpdatePassword()
        {
            // Arrange
            var account = new Account { Id = 1, EmployeeId = 100, PasswordHash = "oldhash" };
            _mockRepository.Setup(r => r.GetByEmployeeIdAsync(100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdatePasswordAsync(100, "newpassword");

            // Assert
            Assert.NotNull(account.ModifiedDate);
            Assert.Equal("System", account.ModifiedBy);
            _mockRepository.Verify(r => r.GetByEmployeeIdAsync(100, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePasswordAsync_WithInvalidEmployeeId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByEmployeeIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdatePasswordAsync(999, "newpassword"));
            _mockRepository.Verify(r => r.GetByEmployeeIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_WithValidId_ShouldReturnAccount()
        {
            // Arrange
            var account = new Account { Id = 1, EmployeeId = 100 };
            _mockRepository.Setup(r => r.GetByEmployeeIdAsync(100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var result = await _service.GetByEmployeeIdAsync(100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.EmployeeId);
            _mockRepository.Verify(r => r.GetByEmployeeIdAsync(100, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByEmployeeIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            // Act
            var result = await _service.GetByEmployeeIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByEmployeeIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldSetDefaultProperties()
        {
            // Arrange
            Account? capturedAccount = null;
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account a, CancellationToken ct) =>
                {
                    capturedAccount = a;
                    return a;
                });

            // Act
            await _service.CreateAsync(100, "password", false);

            // Assert
            Assert.NotNull(capturedAccount);
            Assert.True(capturedAccount.IsActive);
            Assert.Equal("System", capturedAccount.CreatedBy);
            Assert.NotEqual(default(DateTime), capturedAccount.CreatedDate);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            _mockRepository.Setup(r => r.ValidateCredentialsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.ValidateCredentialsAsync(100, "password", false));
        }

        [Fact]
        public async Task CreateAsync_ShouldHashPassword()
        {
            // Arrange
            Account? capturedAccount = null;
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account a, CancellationToken ct) =>
                {
                    capturedAccount = a;
                    return a;
                });

            // Act
            await _service.CreateAsync(100, "plaintext", false);

            // Assert
            Assert.NotNull(capturedAccount);
            Assert.NotEqual("plaintext", capturedAccount.PasswordHash);
            Assert.NotEmpty(capturedAccount.PasswordHash);
        }
    }
}
