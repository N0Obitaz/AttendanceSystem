using System;
using AttendanceSystem.Data;
using AttendanceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Pages.Attendances
{
    public class MarkModel : PageModel
    {
        private readonly AttendanceSystemContext _context;

        public MarkModel(AttendanceSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Student Student { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Student = await _context.Student.FindAsync(id);

            if (Student == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string status)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null)
                return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1️ Create Attendance Record
                var attendance = new Attendance
                {
                    StudentId = student.StudentId,
                    Date = DateTime.Today,
                    Status = status
                };

                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();

                // 2️ Create Transaction Log
                var log = new TransactionLog
                {
                    StudentId = student.StudentId,
                    Action = $"Marked {status}",
                    Timestamp = DateTime.UtcNow
                };

                _context.TransactionLogs.Add(log);
                await _context.SaveChangesAsync();

                //  Commit both inserts
                await transaction.CommitAsync();

                TempData["Message"] = $"Attendance marked for {student.FirstName} {student.LastName}";
                return RedirectToPage("/Students/Index");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw; // or handle error gracefully
            }
        }
    }
}
