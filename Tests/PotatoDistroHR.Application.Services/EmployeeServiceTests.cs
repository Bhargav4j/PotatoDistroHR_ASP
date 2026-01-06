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
    /// Unit tests for EmployeeService
    /// </summary>
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepository;
        private readonly Mock<ILogger<EmployeeService>> _mockLogger;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRepository>();
            _mockLogger = new Mock<ILogger<EmployeeService>>();
            _service = new EmployeeService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "John", LastName = "Doe" },
                new Employee { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };
            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(employees);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Employee>)result).Count);
            _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnEmployee()
        {
            // Arrange
            var employee = new Employee { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John", result.FirstName);
            _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateEmployee()
        {
            // Arrange
            var employee = new Employee { FirstName = "John", LastName = "Doe" };
            var createdEmployee = new Employee { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdEmployee);

            // Act
            var result = await _service.CreateAsync(employee);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.True(result.IsActive);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldUpdateEmployee()
        {
            // Arrange
            var existingEmployee = new Employee { Id = 1, FirstName = "John", LastName = "Doe" };
            var updatedEmployee = new Employee { Id = 1, FirstName = "Jane", LastName = "Smith" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(1, updatedEmployee);

            // Assert
            _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var employee = new Employee { Id = 999, FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(999, employee));
            _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteEmployee()
        {
            // Arrange
            var employee = new Employee { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);
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
                .ReturnsAsync((Employee?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(999));
            _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "John", LastName = "Doe" }
            };
            _mockRepository.Setup(r => r.SearchAsync("John", It.IsAny<CancellationToken>()))
                .ReturnsAsync(employees);

            // Act
            var result = await _service.SearchAsync("John");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _mockRepository.Verify(r => r.SearchAsync("John", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByDepartmentAsync_ShouldReturnEmployeesInDepartment()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, DepartmentId = 10 },
                new Employee { Id = 2, DepartmentId = 10 }
            };
            _mockRepository.Setup(r => r.GetByDepartmentAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employees);

            // Act
            var result = await _service.GetByDepartmentAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Employee>)result).Count);
            _mockRepository.Verify(r => r.GetByDepartmentAsync(10, It.IsAny<CancellationToken>()), Times.Once);
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
            var employee = new Employee { FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee e, CancellationToken ct) => e);

            // Act
            var result = await _service.CreateAsync(employee);

            // Assert
            Assert.True(result.IsActive);
            Assert.NotEqual(default(DateTime), result.CreatedDate);
        }
    }
}
