using Microsoft.AspNetCore.Mvc.RazorPages;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Interfaces.Services;

namespace PotatoDistroHR.Web.Pages.Employees;

public class EmployeesIndexModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesIndexModel> _logger;

    public EmployeesIndexModel(IEmployeeService employeeService, ILogger<EmployeesIndexModel> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();

    public async Task OnGetAsync()
    {
        try
        {
            Employees = await _employeeService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees");
            Employees = new List<Employee>();
        }
    }
}
