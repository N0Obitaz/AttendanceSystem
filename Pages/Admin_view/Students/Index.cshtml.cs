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
using Serilog;

namespace AttendanceSystem.Pages.Students
{

    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;


        public IndexModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;

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

            var cutOffTime = new DateTime(Today.Year, Today.Month, Today.Day, 13, 0, 0);

            
            Student = await _context.Student
                .Include(s => s.Attendances)
                .ToListAsync();

           
            var newAttendances = new List<Attendance>();
            var newLogs = new List<TransactionLog>();

         
            foreach (var student in Student)
            {
           
                bool hasAttendanceToday = student.Attendances != null &&
                                          student.Attendances.Any(a => a.Date.Date == Today);

                if (hasAttendanceToday)
                {
                    continue;
                }

             
                if (now >= cutOffTime)
                {
                   
                    var attendance = new Attendance
                    {
                        StudentId = student.StudentId,
                        Date = Today, 
                        Status = "Absent"
                    };
                    newAttendances.Add(attendance);

               
                    var transactionLog = new TransactionLog
                    {
                        StudentId = student.StudentId,
                        Action = "Marked Absent (Auto)",
                        Timestamp = now
                    };
                    newLogs.Add(transactionLog);
                }
            }

                
            if (newAttendances.Any())
            {
                _context.Attendances.AddRange(newAttendances);
                _context.TransactionLogs.AddRange(newLogs);

                await _context.SaveChangesAsync();

                Log.Information("Marked {Count} students as Absent.", newAttendances.Count);
            }

            // 7. Calculate Totals (OUTSIDE the loop)
            // We re-query the DB to ensure we get the fresh counts including the ones we just added
            TotalPresent = await _context.Attendances
                .CountAsync(a => a.Date.Date == Today && a.Status == "Present");

            TotalAbsent = await _context.Attendances
                .CountAsync(a => a.Date.Date == Today && a.Status == "Absent");
        }
    }
}
