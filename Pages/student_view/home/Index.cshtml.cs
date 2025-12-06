using System.Security.Claims;
using AttendanceSystem.Data;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AttendanceSystem.Pages.student_view.Home
{
    [Authorize(Policy = "StudentOnly")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class IndexModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public IndexModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        public Student? Student { get; set; }
        public int? AttendanceStreak { get; set; }

        public int? LateMarks { get; set; } = 0;

        public int? AbsentMarks { get; set; } = 0;

        public async Task OnGetAsync()
        {
           


            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(User.Identity.Name);

            string username = User.Identity.Name;
            Student = await _context.Student
                .Include(s => s.TransactionLogs.OrderByDescending(t => t.Timestamp))
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.StudentId.ToString() == username);

            if(Student != null || Student.Attendances != null)
            {
                AbsentMarks = Student.Attendances.Count(a => a.Status == "Absent");
                LateMarks = Student.Attendances.Count(a => a.Status == "Late");

                Console.WriteLine($"Absent {AbsentMarks}");
                Console.WriteLine($"Late {LateMarks}");

            }


            await CalculateStreak();
        }



        public async Task<IActionResult> OnPostFilterAsync(DateTime startDate, DateTime endDate)
        {
           
            await CalculateStreak();

            return Page();
        }

        private async Task CalculateStreak()
        {

        var presentDates = _context.Attendances
                .Where(a => a.Status == "Present" && a.StudentId == Student.StudentId)
                .Select(a => a.Date.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            int streak = 0;

            var checkDate = DateTime.Today;

            if (presentDates.Contains(checkDate))
            {
                streak++;
                checkDate = checkDate.AddDays(-1);

                // Only enter the loop if we found Today first
                while (presentDates.Contains(checkDate))
                {
                    streak++;
                    checkDate = checkDate.AddDays(-1);
                }
            }

            AttendanceStreak = streak;
        }
    }
}
