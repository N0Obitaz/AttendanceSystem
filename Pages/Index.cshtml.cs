using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AttendanceSystem.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

   
        public IndexModel(ILogger<IndexModel> logger, AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Student> Students { get; set; }

        public async Task OnGetAsync()
        {
            Students = await _context.Student
                .ToListAsync();
        }
    }
}
