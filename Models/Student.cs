using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.Models
{
    public class Student
    {
        [Key]
        [Display(Name = "Student Number")]
        public int StudentId { get; set; }


        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }


        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();

    }
}
