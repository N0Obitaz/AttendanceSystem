using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AttendanceSystem.Data;
using AttendanceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public CreateModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                bool exists = await _context.Student.AnyAsync(s =>
                   s.StudentId == Student.StudentId ||
                   (s.FirstName == Student.FirstName && s.LastName == Student.LastName)
   );
                if(exists)
                {
                    TempData["ErrorMessage"] = "The Student Already Exists";
                    return Page();
                }
                _context.Student.Add(Student);
                await _context.SaveChangesAsync();

                // Save Enrolling Student Transaction

                var log = new TransactionLog
                {
                    StudentId = Student.StudentId,
                    Action = "Enrolled",
                    Timestamp = DateTime.UtcNow
                };
                _context.TransactionLogs.Add(log);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["Message"] = $"Enrolled New Student {Student.FirstName} {Student.LastName}";
                return RedirectToPage("./Index");


            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }


            
        }
    }
}
