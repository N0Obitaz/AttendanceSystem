using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystem.Models
{
    public class Student
    {
        [Key]
        [Display(Name ="Student Number")]
        public int StudentId { get; set; }

        [Required]
        [StringLength(30)]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
       
        public string? FirstName { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name ="Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Email Address")]
        [StringLength(30)]
        public string? Email { get; set; }
        public string? Username { get; set; }
  
        public string? Password { get; set; }
        public ICollection<Attendance>? Attendances { get; set; }
        public ICollection<TransactionLog>? TransactionLogs { get; set; }

        public string? Institute { get; set; }

    }
}
