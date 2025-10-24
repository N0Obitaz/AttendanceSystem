using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AttendanceSystem.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AttendanceSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        [BindProperty]
        public string? CurrentUser { get; set; }

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
            if (string.IsNullOrEmpty(CurrentUser) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Username and Password are required.");
                return Page();
            }

            // Check for admin
            var admin = await _context.Admin
                .FirstOrDefaultAsync(a => a.Username.ToLower() == CurrentUser.ToLower() && a.Password == Password);

            if (admin != null)
            {
                await SignInUser(admin.Username, "Admin");
                return RedirectToPage("./Admin_view/Dashboard/Index");
            }

            //  Check for student
            string user = new string(CurrentUser.Where(c => c != '-').ToArray());
            var student = await _context.Student
                .FirstOrDefaultAsync(s => s.StudentId.ToString() == user && s.Password == Password);

            if (student != null)
            {
                await SignInUser(student.StudentId.ToString(), "Student");
                return RedirectToPage("./student_view/Home/Index");
            }

            //  Invalid login
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }

        private async Task SignInUser(string username, string role)
        {
            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "CustomSession");
            var principal = new ClaimsPrincipal(identity);

            // Sign in with cookie authentication
            await HttpContext.SignInAsync("CustomSession", principal);
        }
        
    }
}
