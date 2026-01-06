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
    /// Unit tests for DepartmentService
    /// </summary>
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _mockRepository;
        private readonly Mock<ILogger<DepartmentService>> _mockLogger;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            _mockRepository = new Mock<IDepartmentRepository>();
            _mockLogger = new Mock<ILogger<DepartmentService>>();
            _service = new DepartmentService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDepartments()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "HR" },
                new Department { Id = 2, Name = "IT" }
            };
            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(departments);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Department>)result).Count);
            _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnDepartment()
        {
            // Arrange
            var department = new Department { Id = 1, Name = "HR" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(department);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("HR", result.Name);
            _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Department?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateDepartment()
        {
            // Arrange
            var department = new Department { Name = "Engineering" };
            var createdDepartment = new Department { Id = 1, Name = "Engineering" };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDepartment);

            // Act
            var result = await _service.CreateAsync(department);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.True(result.IsActive);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldUpdateDepartment()
        {
            // Arrange
            var existingDepartment = new Department { Id = 1, Name = "HR" };
            var updatedDepartment = new Department { Id = 1, Name = "Human Resources" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(1, updatedDepartment);

            // Assert
            _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var department = new Department { Id = 999, Name = "NonExistent" };
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Department?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(999, department));
            _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteDepartment()
        {
            // Arrange
            var department = new Department { Id = 1, Name = "HR" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(department);
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
                .ReturnsAsync((Department?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(999));
            _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingDepartments()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "Human Resources" }
            };
            _mockRepository.Setup(r => r.SearchAsync("Human", It.IsAny<CancellationToken>()))
                .ReturnsAsync(departments);

            // Act
            var result = await _service.SearchAsync("Human");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _mockRepository.Verify(r => r.SearchAsync("Human", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
        }

        [Fact]
        public async Task CreateAsync_ShouldSetCreatedDateAndIsActive()
        {
            // Arrange
            var department = new Department { Name = "Finance" };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Department d, CancellationToken ct) => d);

            // Act
            var result = await _service.CreateAsync(department);

            // Assert
            Assert.True(result.IsActive);
            Assert.NotEqual(default(DateTime), result.CreatedDate);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSetModifiedDate()
        {
            // Arrange
            var existingDepartment = new Department { Id = 1, Name = "IT" };
            var updatedDepartment = new Department { Id = 1, Name = "Information Technology" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(1, updatedDepartment);

            // Assert
            Assert.NotNull(updatedDepartment.ModifiedDate);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
