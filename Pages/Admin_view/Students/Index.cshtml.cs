using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystem.Data;
using AttendanceSystem.Models;
using AttendanceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

using AttendanceSystem.Extensions;

namespace AttendanceSystem.Pages.Students
{

    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;
        private readonly IDistributedCache _cache;


        public IndexModel(AttendanceSystem.Data.AttendanceSystemContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public DateTime Today { get; set; }
        public IList<Student>? Student { get; set; }

        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }

        public string? Status { get; set; }

        public async Task OnGetAsync()
        {
            var now = DateTime.UtcNow.AddHours(8);
            Today = now.Date;
            var cutOffTime = new DateTime(Today.Year, Today.Month, Today.Day, 12, 0, 0);

            // --- PHASE 1: THE LOGIC (Always Runs) ---
            // We only fetch what we need for the logic to reduce memory usage
            Student = await _context.Student
                .Include(s => s.Attendances)
                .ToListAsync();

            var newAttendances = new List<Attendance>();
            var newLogs = new List<TransactionLog>();

            foreach (var student in Student)
            {
                bool hasAttendanceToday = student.Attendances?.Any(a => a.Date.Date == Today) ?? false;

                if (!hasAttendanceToday && now >= cutOffTime)
                {
                    newAttendances.Add(new Attendance { StudentId = student.StudentId, Date = Today, Status = "Absent" });
                    newLogs.Add(new TransactionLog { StudentId = student.StudentId, Action = "Marked Absent (Auto)", Timestamp = now });
                }
            }

            if (newAttendances.Any())
            {
                _context.Attendances.AddRange(newAttendances);
                _context.TransactionLogs.AddRange(newLogs);
                await _context.SaveChangesAsync();

                // IMPORTANT: If we changed the DB, we MUST clear the cache 
                // so the totals refresh on the next line!
                await _cache.RemoveAsync($"AttendanceTotals_{Today:yyyyMMdd}");
            }

            // --- PHASE 2: THE CACHING (The "Fast" Part) ---
            string cacheKey = $"AttendanceTotals_{Today:yyyyMMdd}";
            var cachedTotals = await _cache.GetRecordAsync<AttendanceSummary>(cacheKey);

            if (cachedTotals == null)
            {
                
                TotalPresent = await _context.Attendances.CountAsync(a => a.Date.Date == Today && a.Status == "Present");
                TotalAbsent = await _context.Attendances.CountAsync(a => a.Date.Date == Today && a.Status == "Absent");

                
                var summary = new AttendanceSummary { Present = TotalPresent, Absent = TotalAbsent };
                await _cache.SetRecordAsync(cacheKey, summary, TimeSpan.FromMinutes(5));
            }
            else
            {
               
                TotalPresent = cachedTotals.Present;
                TotalAbsent = cachedTotals.Absent;
            }
        }
    }

    public class AttendanceSummary
    {
        public int Present { get; set; }
        public int Absent { get; set; }
    }
}
