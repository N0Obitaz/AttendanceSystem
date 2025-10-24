using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace AttendanceSystem.Pages
{
    public class LogoutModel : PageModel
    {
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    // Sign out the user from the "CustomSession" cookie
        //    await HttpContext.SignOutAsync("CustomSession");

        //    // Optional: clear any additional cookies if needed
        //    // Response.Cookies.Delete(".AspNetCore.CustomSession");

        //    // Redirect to login page after logout
        //    return RedirectToPage("/Index");
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            // Sign out the user from the "CustomSession" cookie
            await HttpContext.SignOutAsync("CustomSession");

            // Optional: clear any additional cookies if needed
            // Response.Cookies.Delete(".AspNetCore.CustomSession");

            // Redirect to login page after logout
            return RedirectToPage("/Index");
        }
    }
}
