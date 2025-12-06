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
        public string? Email { get; set; }

        [BindProperty]
        public string? StudentNumber { get; set; }
        [BindProperty]
        public string? FirstName { get; set; }
        [BindProperty]
        public string? LastName { get; set; }

        [BindProperty]
        public string? Institute { get; set; }

        [BindProperty]
        public string? RegisterPassword { get; set; }
        [BindProperty]
        public string? Password { get; set; }

        public IndexModel(ILogger<IndexModel> logger, AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                var claimRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if(claimRole == "Admin")
                {
                    return RedirectToPage("./Admin_view/Dashboard/Index");

                }
                else if (claimRole == "Student")
                {
                    return RedirectToPage("./student_view/Home/Index");
                }
            }


            return Page();
        }

        public async Task<IActionResult> HandleLoginAsync()
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

        public async Task<IActionResult> HandleRegisterAsync()
        {
            try
            {
                Console.WriteLine(RegisterPassword);
                if (!ModelState.IsValid)
                    return Page();

                if (string.IsNullOrWhiteSpace(StudentNumber) ||
                    string.IsNullOrWhiteSpace(FirstName) ||
                    string.IsNullOrWhiteSpace(LastName) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(RegisterPassword)) 
                {
                    ModelState.AddModelError("", "All fields are required.");
                    return Page();
                }
                string output = StudentNumber.Replace("-", "");
                Console.WriteLine($"{output} and {StudentNumber}");


                string user = new string(StudentNumber.Where(c => c != '-').ToArray());
                var student = await _context.Student
                    .FirstOrDefaultAsync(s => s.StudentId.ToString() == user && s.Password == Password);

                if(student == null) {

                    _context.Student.Add(new Student
                    {
                        StudentId = int.Parse(StudentNumber),
                        FirstName = FirstName,
                        LastName = LastName,
                        Institute = Institute,
                        Email = Email,
                        Password = RegisterPassword
                    });

                    await _context.SaveChangesAsync();
                }


                // Success path
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // You can return the Page with an error, or redirect
                return RedirectToPage("./Index");
            }
            return Page();
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (action == "login")
                return await HandleLoginAsync();

            if (action == "register")
                return await HandleRegisterAsync();

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
