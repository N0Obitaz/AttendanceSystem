using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Models;

namespace AttendanceSystem.Data
{
    public class AttendanceSystemContext : DbContext
    {
        public AttendanceSystemContext (DbContextOptions<AttendanceSystemContext> options)
            : base(options)
        {
        }
        
        public DbSet<AttendanceSystem.Models.Student> Student { get; set; } = default!;
        public DbSet<AttendanceSystem.Models.Attendance> Attendances { get; set; }
        public DbSet<AttendanceSystem.Models.TransactionLog> TransactionLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

           
            // Student - Attendance (1-to-Many)
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Attendances)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student -  TransactionLog (1-to-Many)
            modelBuilder.Entity<Student>()
                .HasMany(s => s.TransactionLogs)
                .WithOne(t => t.Student)
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                .HasData(
                new Student { StudentId = 2300185, FirstName = "Charles Bernard", LastName = "Balaguer" },
                new Student { StudentId = 2300192, FirstName = "Ralph Joed", LastName = "Gerente" },
                
                new Student { StudentId = 2300132, FirstName = "Elvis Mar", LastName = "Bayson" },
                new Student { StudentId = 2300553, FirstName = "Knives Benedict", LastName = "Cabarles" },
                new Student { StudentId = 2300592, FirstName = "Mary Joy", LastName = "Collantes" },
                new Student { StudentId = 2300688, FirstName = "Karylle", LastName = "Nicolas" },
                new Student { StudentId = 2305822, FirstName = "Reca Mae", LastName = "Montebon" },
                new Student { StudentId = 2300530, FirstName = "Marie Cris", LastName = "Reboltan" }
                );
        }
    }
}
