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

        public async Task OnGetAsync()
        {

            // load attendance records
            Student = await _context.Student
               .Include(s => s.Attendances)
               .ToListAsync();

            Today = DateTime.UtcNow.AddHours(8).Date;

          

            foreach (var student in Student)
            {
                // Check if student has attendance for today 

                bool hasAttendanceToday = student.Attendances != null && student.Attendances.Any(a => a.Date.Day == Today.Day);

                if (hasAttendanceToday)
                {
                    continue;
                }

                // If no attendace for today, create its absent record

                // Check current time if it is past cutoff time (1:00 PM)
                var currentTime = DateTime.UtcNow.AddHours(8);

                var cutOffTime = new DateTime(Today.Year, Today.Month, Today.Day, 13, 0, 0);

                if (currentTime >= cutOffTime)
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        // Create attendance record
                        var attendance = new Attendance
                        {
                            StudentId = student.StudentId,
                            Date = DateTime.UtcNow.Date,
                            Status = "Absent"
                        };

                        _context.Attendances.Add(attendance);
                        await _context.SaveChangesAsync();

                        // Create transaction log
                        var transactionLog = new TransactionLog
                        {
                            StudentId = student.StudentId,
                            Action = "Marked Present",
                            Timestamp = DateTime.UtcNow
                        };

                        _context.TransactionLogs.Add(transactionLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        Log.Information("Attendance marked Absent for Student ID {StudentId}", student.StudentId);
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
                TotalPresent = await _context.Attendances
               .Where(a => a.Date.Day == Today.Day && a.Status == "Present")
               .CountAsync();

                TotalAbsent = await _context.Attendances
                    .Where(a => a.Date.Day == Today.Day && a.Status == "Absent")
                    .CountAsync();

            }
        }
    }
}
