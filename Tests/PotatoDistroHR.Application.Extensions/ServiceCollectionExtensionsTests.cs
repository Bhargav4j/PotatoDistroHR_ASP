using Xunit;
using Microsoft.Extensions.DependencyInjection;
using PotatoDistroHR.Application.Extensions;
using PotatoDistroHR.Domain.Interfaces.Services;
using PotatoDistroHR.Application.Services;

namespace Tests.PotatoDistroHR.Application.Extensions
{
    /// <summary>
    /// Unit tests for ServiceCollectionExtensions
    /// </summary>
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddApplicationServices_ShouldRegisterEmployeeService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IEmployeeService));
            Assert.NotNull(descriptor);
            Assert.Equal(typeof(EmployeeService), descriptor.ImplementationType);
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterDepartmentService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDepartmentService));
            Assert.NotNull(descriptor);
            Assert.Equal(typeof(DepartmentService), descriptor.ImplementationType);
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterAccountService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IAccountService));
            Assert.NotNull(descriptor);
            Assert.Equal(typeof(AccountService), descriptor.ImplementationType);
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterLeaveRequestService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ILeaveRequestService));
            Assert.NotNull(descriptor);
            Assert.Equal(typeof(LeaveRequestService), descriptor.ImplementationType);
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterPayrollService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IPayrollService));
            Assert.NotNull(descriptor);
            Assert.Equal(typeof(PayrollService), descriptor.ImplementationType);
        }

        [Fact]
        public void AddApplicationServices_ShouldReturnServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddApplicationServices();

            // Assert
            Assert.NotNull(result);
            Assert.Same(services, result);
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterAllServicesAsScoped()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            var employeeServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IEmployeeService));
            Assert.NotNull(employeeServiceDescriptor);
            Assert.Equal(ServiceLifetime.Scoped, employeeServiceDescriptor.Lifetime);

            var departmentServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDepartmentService));
            Assert.NotNull(departmentServiceDescriptor);
            Assert.Equal(ServiceLifetime.Scoped, departmentServiceDescriptor.Lifetime);
        }

        [Fact]
        public void AddApplicationServices_ShouldAllowMultipleCalls()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();
            services.AddApplicationServices();

            // Assert - Should not throw exception
            var descriptors = services.Where(s => s.ServiceType == typeof(IEmployeeService)).ToList();
            Assert.Equal(2, descriptors.Count);
        }
    }
}
