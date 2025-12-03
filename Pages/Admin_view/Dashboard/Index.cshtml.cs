using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using ChartJSCore.Models.ChartJSCore.Models;



namespace AttendanceSystem.Pages.Admin_view.Dashboard
{
    public class DashboardModel : PageModel
    {


        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public DashboardModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [BindProperty]
        public int? Present { get; set; }

        [BindProperty]
        public int? Absent{ get; set; }
        [BindProperty]
        public int? Late { get; set; }

        [BindProperty]
        public int? FilteredPresent { get; set; }
        [BindProperty]
        public int? FilteredAbsent{ get; set; }
        [BindProperty]
        public int? FilteredLate{ get; set; }
        public void OnGet()
        {

            var today = DateTime.UtcNow.Day;

            Present = _context.Attendances
                .Count(a => a.Status == "Present");

            Absent = _context.Attendances
                .Count(a => a.Status == "Absent");

            Late = _context.Attendances
                .Count(a => a.Status == "Late");
        }

        [HttpPost]
        public async Task<IActionResult> OnPostFilterAsync(DateTime startDate, DateTime endDate)
        {
           
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

          
            StartDate = startDate;
            EndDate = endDate;

            if (startDate > endDate)
            {
                ModelState.AddModelError(string.Empty, "Start date must be earlier than end date.");
                return Page();
            }

            var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

            Present = _context.Attendances
                .Count(a => a.Status == "Present");

            Absent = _context.Attendances
                .Count(a => a.Status == "Absent");

            Late = _context.Attendances
                .Count(a => a.Status == "Late");

            FilteredPresent = _context.Attendances
                .Count(a => a.Status == "Present" && a.Date.Date >= startDate.Date && a.Date.Date <= endDate.Date);

            FilteredAbsent = _context.Attendances
                .Count(a => a.Status == "Absent" && a.Date.Date >= startDate.Date && a.Date.Date <= endDate.Date);

            FilteredLate = _context.Attendances
                .Count(a => a.Status == "Late" && a.Date.Date >= startDate.Date && a.Date.Date <= endDate.Date);

            return Page();
        }
    }
}
