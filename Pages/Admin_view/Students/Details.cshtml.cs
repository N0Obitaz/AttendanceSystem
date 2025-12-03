using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Models;

namespace AttendanceSystem.Pages.Students
{
    public class DetailsModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public DetailsModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        public Student Student { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }
            else
            {
                Student = student;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // 1. Fetch Student and Include Attendances
            var student = await _context.Student
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            // 2. Define "Today" correctly (Sync with your Timezone)
            // If you are in PH, ensure you add the offset, or use the same logic as your Create method.
            var todayDate = DateTime.UtcNow.AddHours(8).Date;

            // 3. Find the specific record by comparing the FULL Date (Year + Month + Day)
            var attendanceToDelete = student.Attendances
                .FirstOrDefault(a => a.Date.Date == todayDate);

            // 4. Remove and Save
            if (attendanceToDelete != null)
            {
                // It is safer to remove directly from the Context Set 
                // to ensure EF tracks the deletion correctly.
                _context.Attendances.Remove(attendanceToDelete);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
