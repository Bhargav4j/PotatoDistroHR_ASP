using Microsoft.AspNetCore.Mvc.RazorPages;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Interfaces.Services;

namespace PotatoDistroHR.Web.Pages.Departments;

public class DepartmentsIndexModel : PageModel
{
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<DepartmentsIndexModel> _logger;

    public DepartmentsIndexModel(IDepartmentService departmentService, ILogger<DepartmentsIndexModel> logger)
    {
        _departmentService = departmentService;
        _logger = logger;
    }

    public IEnumerable<Department> Departments { get; set; } = new List<Department>();

    public async Task OnGetAsync()
    {
        try
        {
            Departments = await _departmentService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            Departments = new List<Department>();
        }
    }
}
