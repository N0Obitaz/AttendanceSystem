using System;
using System.Collections.Generic;
using System.Linq;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using ChartJSCore.Models.ChartJSCore.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AttendanceSystem.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace AttendanceSystem.Pages.Admin_view.AttendanceMatrix
{
    public class MatricesModel : PageModel
    {
        public Chart? AttendanceChart { get; set; }

        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        public MatricesModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }
        public async Task OnGetAsync(int? id)
        {

            if (id == null)
            {
                id = 2300185;
            }
            var student = await _context.Student
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(m => m.StudentId == id);

            if (student == null) 
                return;



            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            var data = new ChartJSCore.Models.Data();

            data.Labels = new List<string>() { "Sep", "Oct", "Nov", "Dec" };
            

            LineDataset dataset = new LineDataset()
            {
                Label = "Attendance Summary",
                Data = new List<double?> { 21, 20, 23, 24 },
                Fill = "false",
                Tension = 0.1,
                BackgroundColor = new List<ChartColor> { ChartColor.FromRgba(75, 192, 192, 0.4) },
                BorderColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",


            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            ViewData["chart"] = chart;



            // Attendances Doughnut Chart
            Chart doughnutChart = new Chart();

            doughnutChart.Type = Enums.ChartType.Doughnut;

            var doughnutData = new ChartJSCore.Models.Data();

            doughnutData.Labels = new List<string>() { "Present", "Absent", "Late" };

            DoughnutDataset doughnutDataset = new DoughnutDataset()
            {
                Label = "Attendance Breakdown",
                Data = new List<double?> { student.Attendances.Count(a => a.Status == "Present"),
                                           student.Attendances.Count(a => a.Status == "Absent"),
                                           student.Attendances.Count(a => a.Status == "Late") },
                BackgroundColor = new List<ChartColor> { ChartColor.FromRgba(75, 192, 192, 0.4) , ChartColor.FromRgba(255, 205, 86, 0.8) , ChartColor.FromRgba(255, 99, 132, 0.8) }
            };
            doughnutData.Datasets = new List<Dataset>();
            doughnutData.Datasets.Add(doughnutDataset);

            doughnutChart.Data = doughnutData;

            ViewData["doughnutChart"] = doughnutChart;

        }
    }
}
