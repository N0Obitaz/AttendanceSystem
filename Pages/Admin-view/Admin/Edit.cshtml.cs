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

namespace AttendanceSystem.Pages.Admin
{
    public class EditModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public EditModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Administrator Admin { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin =  await _context.Admin.FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }
            Admin = admin;
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

            _context.Attach(Admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(Admin.AdminId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.AdminId == id);
        }
    }
}
