using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AttendanceSystem.Data;
using AttendanceSystem.Models;

namespace AttendanceSystem.Pages.Admin
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
        public Administrator Admin { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Admin.Add(Admin);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
