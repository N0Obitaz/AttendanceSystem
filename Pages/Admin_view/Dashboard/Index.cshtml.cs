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
        public void OnGet()
        {

        }
    }
}
