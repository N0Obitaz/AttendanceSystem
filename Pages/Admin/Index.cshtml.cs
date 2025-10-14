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
    public class IndexModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public IndexModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        public IList<Administrator> Admin { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Admin = await _context.Admin.ToListAsync();
        }
    }
}
