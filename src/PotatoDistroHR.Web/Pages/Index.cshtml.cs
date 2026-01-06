using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PotatoDistroHR.Web.Pages;

public class IndexModel : PageModel
{
    public bool IsAuthenticated { get; set; }

    public void OnGet()
    {
        IsAuthenticated = HttpContext.Session.GetInt32("UserID").HasValue;
    }
}
