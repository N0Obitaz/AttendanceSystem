using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Models;

namespace AttendanceSystem.Pages.Students
{
    public class EditModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public EditModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Student Student { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student =  await _context.Student.FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }
            Student = student;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Attach(Student).State = EntityState.Modified;

                try
                {   

                    await _context.SaveChangesAsync();

                    var log = new TransactionLog
                    {
                        StudentId = Student.StudentId,
                        Action = "Edited",
                        Timestamp = DateTime.UtcNow
                    };

                    _context.TransactionLogs.Add(log);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["Message"] = $"Edited Done {Student.FirstName} {Student.LastName}";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(Student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToPage("./Index");
            } catch
            {
                await transaction.RollbackAsync();
                throw;
            }
           
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.StudentId == id);
        }
    }
}
