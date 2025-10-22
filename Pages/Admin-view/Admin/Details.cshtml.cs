using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Models;

namespace AttendanceSystem.Pages.Admin
{
    public class DetailsModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public DetailsModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        public Administrator Admin { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }
            else
            {
                Admin = admin;
            }
            return Page();
        }
    }
}
