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

        public string Role { get; set; } = "Student";

        public IndexModel(ILogger<IndexModel> logger, AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Administrator> Admin { get; set; }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync()
        {

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Username and Password are required.");
                return Page();
            }
            
            var admin = await _context.Admin
                .FirstOrDefaultAsync(a => a.Username.ToLower() == Username.ToLower() && a.Password == Password);

            if (admin.Role != "Admin")
            {
                var student = await _context.Student
                    .FirstOrDefaultAsync(s => s.Username.ToLower() == Username.ToLower() && s.Password == Password);

                if (student != null)
                {
                    return RedirectToPage("/ScanQR/ScanQR");
                } else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                }
            }
            if (admin != null)
            {
                // Successful login - redirect to existing admin page
                return RedirectToPage("/Students/Index"); // Make sure this page exists
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }
        }

    }
}
