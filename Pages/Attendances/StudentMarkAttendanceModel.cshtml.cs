using AttendanceSystem.Data;
using AttendanceSystem.Models;
using AttendanceSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Pages.Attendances
{
    public class StudentMarkAttendanceModel : PageModel
    {
        private readonly AttendanceSystemContext _context;

        public StudentMarkAttendanceModel(AttendanceSystemContext context)
        {
            _context = context;
        }
        public string Message { get; set; } = "";

        public async Task<IActionResult> OnGetRecordAsync(string token, int id, string status)
        {
            var tokenService = new TokenService();
            if (tokenService.ValidateToken(token, out var email))
            {
            

                var student = await _context.Student.FindAsync(id);
                if (student == null)
                {

                    Message = $"Student Not Found";
                    return NotFound();
                }
                var today = DateTime.UtcNow.AddHours(8);

                bool alreadyRecorded = await _context.Attendances
                    .AnyAsync(a => a.StudentId == student.StudentId && a.Date.Date == today);

                var attendance = new Attendance
                {
                    StudentId = student.StudentId,
                    Date = DateTime.UtcNow.AddHours(8),
                    Status = status
                };

                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
                // valid
                Message = $"Attendance recorded for {email}";
                return Page();
            }

            // expired
            Message = "This link has expired or is invalid.";
            return Page();
        }



    }
}
