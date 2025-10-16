using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystem.Data;
using AttendanceSystem.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AttendanceSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Pages.Students
{
    [CustomAuthorize]

    public class IndexModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;
       
        public IndexModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;

          
        }
        public DateTime Today { get; set; }
        public IList<Student>? Student { get;set; }

        public async Task OnGetAsync()
        {

            // load attendance records
            Student = await _context.Student
               .Include(s => s.Attendances) 
               .ToListAsync();

            
        }
    }
}
