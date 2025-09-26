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
                new Student { StudentId = 2, FirstName = "Ralph Joed", LastName = "Gerente" },
                new Student { StudentId = 3, FirstName = "Charles Bernard", LastName = "Balaguer" }
                );
        }
    }
}
