using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace AttendanceSystem.Models
{
    public class TransactionLog
    {
        [Key]
        [Display(Name = "Transaction ID")]
        public int TransactionId { get; set; }
        public string? Action { get; set; }

        public DateTime Timestamp { get; set; }

        
        public int StudentId { get; set; }

        [Display(Name = "Student Number")]
        public Student? Student { get; set; }


    }
}
