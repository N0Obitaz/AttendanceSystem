using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystem.Models
{
    public class Student
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] // disables auto-increment
        public int StudentId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<Attendance>? Attendances { get; set; }
        public ICollection<TransactionLog>? TransactionLogs { get; set; }

    }
}
