using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystem.Models
{
    public class Administrator
    {

        [Key]
        public int AdminId { get; set; }
        [Required]

        public string? Username { get; set; }
        public string? Password { get; set; }

        public string? Role { get; set; }

        public string? Email { get; set; }
    }
}
