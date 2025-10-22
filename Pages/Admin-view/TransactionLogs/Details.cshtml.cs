using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Models;

namespace AttendanceSystem.Pages.TransactionLogs
{
    public class DetailsModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public DetailsModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        public TransactionLog TransactionLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionlog = await _context.TransactionLogs.FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transactionlog == null)
            {
                return NotFound();
            }
            else
            {
                TransactionLog = transactionlog;
            }
            return Page();
        }
    }
}
