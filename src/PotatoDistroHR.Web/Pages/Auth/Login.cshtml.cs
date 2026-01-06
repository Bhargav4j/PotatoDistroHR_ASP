using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PotatoDistroHR.Domain.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace PotatoDistroHR.Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IAccountService _accountService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IAccountService accountService, ILogger<LoginModel> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Employee ID is required")]
    public int EmployeeId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool IsAdmin { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var account = await _accountService.ValidateCredentialsAsync(EmployeeId, Password, IsAdmin);

            if (account != null)
            {
                HttpContext.Session.SetInt32("UserID", account.EmployeeId);
                HttpContext.Session.SetString("UserFullName", $"{account.Employee.FirstName} {account.Employee.LastName}");
                HttpContext.Session.SetString("IsAdmin", account.IsAdmin.ToString().ToLower());

                _logger.LogInformation("User {EmployeeId} logged in successfully", EmployeeId);

                if (IsAdmin)
                {
                    return RedirectToPage("/Employees/Index");
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }
            else
            {
                ErrorMessage = "Invalid employee ID or password";
                _logger.LogWarning("Failed login attempt for employee ID: {EmployeeId}", EmployeeId);
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for employee ID: {EmployeeId}", EmployeeId);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }
}
