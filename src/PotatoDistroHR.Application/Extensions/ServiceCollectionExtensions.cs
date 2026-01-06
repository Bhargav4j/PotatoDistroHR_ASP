using Microsoft.Extensions.DependencyInjection;
using PotatoDistroHR.Application.Services;
using PotatoDistroHR.Domain.Interfaces.Services;

namespace PotatoDistroHR.Application.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        services.AddScoped<IPayrollService, PayrollService>();

        return services;
    }
}
