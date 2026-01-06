using Xunit;
using System;
using PotatoDistroHR.Domain.Entities;

namespace Tests.PotatoDistroHR.Domain.Entities
{
    /// <summary>
    /// Unit tests for Payroll entity
    /// </summary>
    public class PayrollTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var payroll = new Payroll();

            // Assert
            Assert.Equal(0, payroll.Id);
            Assert.Equal(0, payroll.EmployeeId);
            Assert.Equal(0m, payroll.BaseSalary);
            Assert.Equal(0m, payroll.Bonus);
            Assert.Equal(0m, payroll.Deductions);
            Assert.Equal(0m, payroll.NetSalary);
            Assert.Equal("Pending", payroll.Status);
            Assert.True(payroll.IsActive);
            Assert.Equal(string.Empty, payroll.CreatedBy);
        }

        [Fact]
        public void Id_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            int expectedId = 100;

            // Act
            payroll.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, payroll.Id);
        }

        [Fact]
        public void EmployeeId_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            int expectedEmployeeId = 50;

            // Act
            payroll.EmployeeId = expectedEmployeeId;

            // Assert
            Assert.Equal(expectedEmployeeId, payroll.EmployeeId);
        }

        [Fact]
        public void BaseSalary_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            decimal expectedSalary = 5000.00m;

            // Act
            payroll.BaseSalary = expectedSalary;

            // Assert
            Assert.Equal(expectedSalary, payroll.BaseSalary);
        }

        [Fact]
        public void Bonus_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            decimal expectedBonus = 500.00m;

            // Act
            payroll.Bonus = expectedBonus;

            // Assert
            Assert.Equal(expectedBonus, payroll.Bonus);
        }

        [Fact]
        public void Deductions_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            decimal expectedDeductions = 250.00m;

            // Act
            payroll.Deductions = expectedDeductions;

            // Assert
            Assert.Equal(expectedDeductions, payroll.Deductions);
        }

        [Fact]
        public void NetSalary_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            decimal expectedNetSalary = 5250.00m;

            // Act
            payroll.NetSalary = expectedNetSalary;

            // Assert
            Assert.Equal(expectedNetSalary, payroll.NetSalary);
        }

        [Fact]
        public void PayPeriodStart_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            var expectedDate = new DateTime(2026, 1, 1);

            // Act
            payroll.PayPeriodStart = expectedDate;

            // Assert
            Assert.Equal(expectedDate, payroll.PayPeriodStart);
        }

        [Fact]
        public void PayPeriodEnd_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            var expectedDate = new DateTime(2026, 1, 31);

            // Act
            payroll.PayPeriodEnd = expectedDate;

            // Assert
            Assert.Equal(expectedDate, payroll.PayPeriodEnd);
        }

        [Fact]
        public void PayDate_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            var expectedDate = new DateTime(2026, 2, 5);

            // Act
            payroll.PayDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, payroll.PayDate);
        }

        [Fact]
        public void Status_ShouldDefaultToPending()
        {
            // Arrange & Act
            var payroll = new Payroll();

            // Assert
            Assert.Equal("Pending", payroll.Status);
        }

        [Fact]
        public void Status_ShouldSetToProcessed()
        {
            // Arrange
            var payroll = new Payroll();

            // Act
            payroll.Status = "Processed";

            // Assert
            Assert.Equal("Processed", payroll.Status);
        }

        [Fact]
        public void Status_ShouldSetToPaid()
        {
            // Arrange
            var payroll = new Payroll();

            // Act
            payroll.Status = "Paid";

            // Assert
            Assert.Equal("Paid", payroll.Status);
        }

        [Fact]
        public void IsActive_ShouldDefaultToTrue()
        {
            // Arrange & Act
            var payroll = new Payroll();

            // Assert
            Assert.True(payroll.IsActive);
        }

        [Fact]
        public void IsActive_ShouldSetToFalse()
        {
            // Arrange
            var payroll = new Payroll();

            // Act
            payroll.IsActive = false;

            // Assert
            Assert.False(payroll.IsActive);
        }

        [Fact]
        public void CreatedDate_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            var expectedDate = DateTime.UtcNow;

            // Act
            payroll.CreatedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, payroll.CreatedDate);
        }

        [Fact]
        public void ModifiedDate_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            var expectedDate = DateTime.UtcNow;

            // Act
            payroll.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, payroll.ModifiedDate);
        }

        [Fact]
        public void ModifiedDate_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var payroll = new Payroll();

            // Assert
            Assert.Null(payroll.ModifiedDate);
        }

        [Fact]
        public void CreatedBy_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            string expectedUser = "payroll_admin";

            // Act
            payroll.CreatedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, payroll.CreatedBy);
        }

        [Fact]
        public void ModifiedBy_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            string expectedUser = "admin";

            // Act
            payroll.ModifiedBy = expectedUser;

            // Assert
            Assert.Equal(expectedUser, payroll.ModifiedBy);
        }

        [Fact]
        public void ModifiedBy_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var payroll = new Payroll();

            // Assert
            Assert.Null(payroll.ModifiedBy);
        }

        [Fact]
        public void Employee_ShouldSetAndGet()
        {
            // Arrange
            var payroll = new Payroll();
            var employee = new Employee { Id = 1, FirstName = "John", LastName = "Doe" };

            // Act
            payroll.Employee = employee;

            // Assert
            Assert.NotNull(payroll.Employee);
            Assert.Equal(1, payroll.Employee.Id);
        }

        [Fact]
        public void Payroll_ShouldSupportFullPropertyAssignment()
        {
            // Arrange
            var payroll = new Payroll
            {
                Id = 1,
                EmployeeId = 10,
                BaseSalary = 6000.00m,
                Bonus = 1000.00m,
                Deductions = 500.00m,
                NetSalary = 6500.00m,
                PayPeriodStart = new DateTime(2026, 1, 1),
                PayPeriodEnd = new DateTime(2026, 1, 31),
                PayDate = new DateTime(2026, 2, 5),
                Status = "Paid",
                IsActive = true,
                CreatedBy = "system"
            };

            // Assert
            Assert.Equal(1, payroll.Id);
            Assert.Equal(10, payroll.EmployeeId);
            Assert.Equal(6000.00m, payroll.BaseSalary);
            Assert.Equal(1000.00m, payroll.Bonus);
            Assert.Equal(500.00m, payroll.Deductions);
            Assert.Equal(6500.00m, payroll.NetSalary);
            Assert.Equal(new DateTime(2026, 1, 1), payroll.PayPeriodStart);
            Assert.Equal(new DateTime(2026, 1, 31), payroll.PayPeriodEnd);
            Assert.Equal(new DateTime(2026, 2, 5), payroll.PayDate);
            Assert.Equal("Paid", payroll.Status);
            Assert.True(payroll.IsActive);
            Assert.Equal("system", payroll.CreatedBy);
        }

        [Fact]
        public void NetSalary_Calculation_ShouldBeCorrect()
        {
            // Arrange
            var payroll = new Payroll
            {
                BaseSalary = 5000.00m,
                Bonus = 500.00m,
                Deductions = 250.00m
            };

            // Act
            payroll.NetSalary = payroll.BaseSalary + payroll.Bonus - payroll.Deductions;

            // Assert
            Assert.Equal(5250.00m, payroll.NetSalary);
        }

        [Fact]
        public void BaseSalary_ShouldAcceptZeroValue()
        {
            // Arrange
            var payroll = new Payroll();

            // Act
            payroll.BaseSalary = 0m;

            // Assert
            Assert.Equal(0m, payroll.BaseSalary);
        }

        [Fact]
        public void Bonus_ShouldAcceptZeroValue()
        {
            // Arrange
            var payroll = new Payroll();

            // Act
            payroll.Bonus = 0m;

            // Assert
            Assert.Equal(0m, payroll.Bonus);
        }

        [Fact]
        public void Deductions_ShouldAcceptZeroValue()
        {
            // Arrange
            var payroll = new Payroll();

            // Act
            payroll.Deductions = 0m;

            // Assert
            Assert.Equal(0m, payroll.Deductions);
        }
    }
}
