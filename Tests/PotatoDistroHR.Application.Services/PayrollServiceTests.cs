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
    /// Unit tests for PayrollService
    /// </summary>
    public class PayrollServiceTests
    {
        private readonly Mock<IPayrollRepository> _mockPayrollRepository;
        private readonly Mock<IEmployeeRepository> _mockEmployeeRepository;
        private readonly Mock<ILogger<PayrollService>> _mockLogger;
        private readonly PayrollService _service;

        public PayrollServiceTests()
        {
            _mockPayrollRepository = new Mock<IPayrollRepository>();
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockLogger = new Mock<ILogger<PayrollService>>();
            _service = new PayrollService(_mockPayrollRepository.Object, _mockEmployeeRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPayrolls()
        {
            // Arrange
            var payrolls = new List<Payroll>
            {
                new Payroll { Id = 1, EmployeeId = 10 },
                new Payroll { Id = 2, EmployeeId = 20 }
            };
            _mockPayrollRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(payrolls);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Payroll>)result).Count);
            _mockPayrollRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnPayroll()
        {
            // Arrange
            var payroll = new Payroll { Id = 1, EmployeeId = 10 };
            _mockPayrollRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payroll);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _mockPayrollRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCalculateNetSalary()
        {
            // Arrange
            var payroll = new Payroll
            {
                EmployeeId = 10,
                BaseSalary = 5000m,
                Bonus = 500m,
                Deductions = 250m
            };
            _mockPayrollRepository.Setup(r => r.AddAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payroll p, CancellationToken ct) => p);

            // Act
            var result = await _service.CreateAsync(payroll);

            // Assert
            Assert.Equal(5250m, result.NetSalary);
            Assert.True(result.IsActive);
            _mockPayrollRepository.Verify(r => r.AddAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldUpdatePayrollAndCalculateNetSalary()
        {
            // Arrange
            var existingPayroll = new Payroll { Id = 1, EmployeeId = 10 };
            var updatedPayroll = new Payroll
            {
                Id = 1,
                EmployeeId = 10,
                BaseSalary = 6000m,
                Bonus = 1000m,
                Deductions = 500m
            };
            _mockPayrollRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPayroll);
            _mockPayrollRepository.Setup(r => r.UpdateAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(1, updatedPayroll);

            // Assert
            Assert.Equal(6500m, updatedPayroll.NetSalary);
            _mockPayrollRepository.Verify(r => r.UpdateAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var payroll = new Payroll { Id = 999 };
            _mockPayrollRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payroll?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(999, payroll));
            _mockPayrollRepository.Verify(r => r.UpdateAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeletePayroll()
        {
            // Arrange
            var payroll = new Payroll { Id = 1, EmployeeId = 10 };
            _mockPayrollRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payroll);
            _mockPayrollRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(1);

            // Assert
            _mockPayrollRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _mockPayrollRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _mockPayrollRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payroll?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(999));
            _mockPayrollRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_ShouldReturnEmployeePayrolls()
        {
            // Arrange
            var payrolls = new List<Payroll>
            {
                new Payroll { Id = 1, EmployeeId = 10 },
                new Payroll { Id = 2, EmployeeId = 10 }
            };
            _mockPayrollRepository.Setup(r => r.GetByEmployeeIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payrolls);

            // Act
            var result = await _service.GetByEmployeeIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Payroll>)result).Count);
            _mockPayrollRepository.Verify(r => r.GetByEmployeeIdAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByPayPeriodAsync_ShouldReturnPayrollsInPeriod()
        {
            // Arrange
            var startDate = new DateTime(2026, 1, 1);
            var endDate = new DateTime(2026, 1, 31);
            var payrolls = new List<Payroll>
            {
                new Payroll { Id = 1, PayPeriodStart = startDate, PayPeriodEnd = endDate }
            };
            _mockPayrollRepository.Setup(r => r.GetByPayPeriodAsync(startDate, endDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payrolls);

            // Act
            var result = await _service.GetByPayPeriodAsync(startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _mockPayrollRepository.Verify(r => r.GetByPayPeriodAsync(startDate, endDate, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GeneratePayrollAsync_WithValidEmployee_ShouldGeneratePayroll()
        {
            // Arrange
            var employee = new Employee { Id = 10, Salary = 5000m };
            var payPeriodStart = new DateTime(2026, 1, 1);
            var payPeriodEnd = new DateTime(2026, 1, 31);
            _mockEmployeeRepository.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);
            _mockPayrollRepository.Setup(r => r.AddAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payroll p, CancellationToken ct) => p);

            // Act
            var result = await _service.GeneratePayrollAsync(10, payPeriodStart, payPeriodEnd);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.EmployeeId);
            Assert.Equal(5000m, result.BaseSalary);
            Assert.Equal(5000m, result.NetSalary);
            Assert.Equal("Pending", result.Status);
            Assert.Equal(payPeriodStart, result.PayPeriodStart);
            Assert.Equal(payPeriodEnd, result.PayPeriodEnd);
            _mockPayrollRepository.Verify(r => r.AddAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GeneratePayrollAsync_WithInvalidEmployee_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _mockEmployeeRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _service.GeneratePayrollAsync(999, DateTime.Now, DateTime.Now));
            _mockPayrollRepository.Verify(r => r.AddAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GeneratePayrollAsync_ShouldSetPayDateSevenDaysAfterPeriodEnd()
        {
            // Arrange
            var employee = new Employee { Id = 10, Salary = 5000m };
            var payPeriodStart = new DateTime(2026, 1, 1);
            var payPeriodEnd = new DateTime(2026, 1, 31);
            var expectedPayDate = payPeriodEnd.AddDays(7);
            _mockEmployeeRepository.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);
            _mockPayrollRepository.Setup(r => r.AddAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payroll p, CancellationToken ct) => p);

            // Act
            var result = await _service.GeneratePayrollAsync(10, payPeriodStart, payPeriodEnd);

            // Assert
            Assert.Equal(expectedPayDate, result.PayDate);
        }

        [Fact]
        public async Task CreateAsync_WithZeroBonus_ShouldCalculateCorrectNetSalary()
        {
            // Arrange
            var payroll = new Payroll
            {
                BaseSalary = 5000m,
                Bonus = 0m,
                Deductions = 250m
            };
            _mockPayrollRepository.Setup(r => r.AddAsync(It.IsAny<Payroll>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payroll p, CancellationToken ct) => p);

            // Act
            var result = await _service.CreateAsync(payroll);

            // Assert
            Assert.Equal(4750m, result.NetSalary);
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            _mockPayrollRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
        }
    }
}
