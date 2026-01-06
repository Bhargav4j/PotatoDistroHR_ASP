using Xunit;
using Microsoft.EntityFrameworkCore;
using PotatoDistroHR.Infrastructure.Data;
using PotatoDistroHR.Domain.Entities;

namespace Tests.PotatoDistroHR.Infrastructure.Data
{
    /// <summary>
    /// Unit tests for ApplicationDbContext
    /// </summary>
    public class ApplicationDbContextTests
    {
        private DbContextOptions<ApplicationDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void Constructor_ShouldInitializeContext()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Employees);
            Assert.NotNull(context.Departments);
            Assert.NotNull(context.Accounts);
            Assert.NotNull(context.LeaveRequests);
            Assert.NotNull(context.Payrolls);
        }

        [Fact]
        public void Employees_DbSet_ShouldBeAccessible()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.Employees);
        }

        [Fact]
        public void Departments_DbSet_ShouldBeAccessible()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.Departments);
        }

        [Fact]
        public void Accounts_DbSet_ShouldBeAccessible()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.Accounts);
        }

        [Fact]
        public void LeaveRequests_DbSet_ShouldBeAccessible()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.LeaveRequests);
        }

        [Fact]
        public void Payrolls_DbSet_ShouldBeAccessible()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.Payrolls);
        }

        [Fact]
        public async Task AddEmployee_ShouldPersistToDatabase()
        {
            // Arrange
            var options = CreateOptions();
            var employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1990, 1, 1),
                DepartmentId = 1
            };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Employees.Add(employee);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var savedEmployee = await context.Employees.FirstOrDefaultAsync();
                Assert.NotNull(savedEmployee);
                Assert.Equal("John", savedEmployee.FirstName);
                Assert.Equal("Doe", savedEmployee.LastName);
            }
        }

        [Fact]
        public async Task AddDepartment_ShouldPersistToDatabase()
        {
            // Arrange
            var options = CreateOptions();
            var department = new Department { Name = "IT", Hotline = "555-1234" };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Departments.Add(department);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var savedDepartment = await context.Departments.FirstOrDefaultAsync();
                Assert.NotNull(savedDepartment);
                Assert.Equal("IT", savedDepartment.Name);
            }
        }

        [Fact]
        public async Task AddAccount_ShouldPersistToDatabase()
        {
            // Arrange
            var options = CreateOptions();
            var account = new Account { EmployeeId = 1, PasswordHash = "hash123", IsAdmin = false };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Accounts.Add(account);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var savedAccount = await context.Accounts.FirstOrDefaultAsync();
                Assert.NotNull(savedAccount);
                Assert.Equal(1, savedAccount.EmployeeId);
            }
        }

        [Fact]
        public async Task AddLeaveRequest_ShouldPersistToDatabase()
        {
            // Arrange
            var options = CreateOptions();
            var leaveRequest = new LeaveRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(5),
                Reason = "Vacation"
            };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.LeaveRequests.Add(leaveRequest);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var savedLeaveRequest = await context.LeaveRequests.FirstOrDefaultAsync();
                Assert.NotNull(savedLeaveRequest);
                Assert.Equal("Vacation", savedLeaveRequest.Reason);
            }
        }

        [Fact]
        public async Task AddPayroll_ShouldPersistToDatabase()
        {
            // Arrange
            var options = CreateOptions();
            var payroll = new Payroll
            {
                EmployeeId = 1,
                BaseSalary = 5000m,
                NetSalary = 5000m
            };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Payrolls.Add(payroll);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var savedPayroll = await context.Payrolls.FirstOrDefaultAsync();
                Assert.NotNull(savedPayroll);
                Assert.Equal(5000m, savedPayroll.BaseSalary);
            }
        }

        [Fact]
        public async Task Context_ShouldSupportMultipleEntities()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Departments.Add(new Department { Name = "HR" });
                context.Employees.Add(new Employee { FirstName = "Jane", LastName = "Smith", DepartmentId = 1 });
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var departmentCount = await context.Departments.CountAsync();
                var employeeCount = await context.Employees.CountAsync();
                Assert.Equal(1, departmentCount);
                Assert.Equal(1, employeeCount);
            }
        }

        [Fact]
        public void Context_ShouldBeDisposable()
        {
            // Arrange
            var options = CreateOptions();
            ApplicationDbContext? context = null;

            // Act
            context = new ApplicationDbContext(options);
            context.Dispose();

            // Assert - No exception should be thrown
            Assert.NotNull(context);
        }
    }
}
