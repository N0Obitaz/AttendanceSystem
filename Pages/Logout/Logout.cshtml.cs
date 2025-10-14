using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AttendanceSystem.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear all session data
            HttpContext.Session.Remove("IsLoggedIn");
            HttpContext.Session.Remove("Username");

            // Optional: Clear the entire session
            HttpContext.Session.Clear();

            return Page();
        }
    }
}