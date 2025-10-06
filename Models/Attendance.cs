using System.ComponentModel.DataAnnotations;


namespace AttendanceSystem.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        public int StudentId { get; set; }
        public DateTime Date { get; set; }
       

        public string? Status { get; set; }

        public Student? Student { get; set; }

    }
}
