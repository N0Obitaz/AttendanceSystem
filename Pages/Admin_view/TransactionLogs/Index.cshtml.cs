using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace AttendanceSystem.Pages.TransactionLogs
{
    public class IndexModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;
        private readonly IDistributedCache _cache;
        public IndexModel(AttendanceSystem.Data.AttendanceSystemContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IList<TransactionLog> TransactionLog { get;set; } = default!;

        public async Task OnGetAsync()
        {
            TransactionLog = await _context.TransactionLogs
                .Include(t => t.Student)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }
    }
}
