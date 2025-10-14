using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AttendanceSystem.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public IndexModel(ILogger<IndexModel> logger, AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Username and Password are required.");
                return Page();
            }

            // First check if it's an admin
            var admin = await _context.Admin
                .FirstOrDefaultAsync(a => a.Username.ToLower() == Username.ToLower() && a.Password == Password);

            if (admin != null)
            {
                // Admin login successful
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("Username", Username);
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToPage("/Students/Index");
            }

            // If not admin, check if it's a student
            var student = await _context.Student
                .FirstOrDefaultAsync(s => s.Username.ToLower() == Username.ToLower() && s.Password == Password);

            if (student != null)
            {
                // Student login successful
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("Username", Username);
                HttpContext.Session.SetString("Role", "Student");
                return RedirectToPage("/ScanQR/ScanQR");
            }

            // If neither admin nor student found
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }
    }
}