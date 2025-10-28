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
        
        public List<Chart> AttendanceChartList { get; set; } = new List<Chart>();
            
        public string CenterValue { get; set; }

        public MatricesModel(AttendanceSystem.Data.AttendanceSystemContext context)
        {
            _context = context;
        }

        //public async Chart ChartAsync(int? id)
        //{

        //    Chart chart = new Chart();

        //    if (id == null)
        //    {
        //        return null;
        //    }


        //    var student = await _context.Student
        //        .Include(s => s.Attendances)
        //        .FirstOrDefaultAsync(m => m.StudentId == id);
        //    if(student == null)
        //    {
        //        return chart;
        //    }
        //    return chart;
        //}
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



            Chart lineChart = new Chart();

            lineChart.Type = Enums.ChartType.Line;

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

            lineChart.Data = data;

            ViewData["chart"] = lineChart;



            // Attendances Doughnut Chart
            Chart doughnutChart = new Chart();

            doughnutChart.Type = Enums.ChartType.Doughnut;

            var doughnutData = new ChartJSCore.Models.Data();

            doughnutData.Labels = new List<string>() { "Present", "Absent", "Late" };

            DoughnutDataset doughnutDataset = new DoughnutDataset()
            {
                Label = "Attendance Breakdown",
                Data = new List<double?>
    {
        student.Attendances.Count(a => a.Status == "Present"),
        student.Attendances.Count(a => a.Status == "Absent"),
        student.Attendances.Count(a => a.Status == "Late")
    },
                BackgroundColor = new List<ChartColor>
    {
        ChartColor.FromRgba(0, 163, 108, 0.9),  
        ChartColor.FromRgba(215, 0, 64, 0.8),   
        ChartColor.FromRgba(255, 205, 86, 0.8)   
    },
                BorderColor = new List<ChartColor>
    {
        ChartColor.FromRgba(255, 255, 255, 1),
        ChartColor.FromRgba(255, 255, 255, 1),
        ChartColor.FromRgba(255, 255, 255, 1)
    },
                BorderWidth = new List<int> { 2, 2, 2 },
                HoverOffset = 25
            };

            doughnutData.Datasets = new List<Dataset>();
            doughnutData.Datasets.Add(doughnutDataset);

            doughnutChart.Data = doughnutData;

            int total = student.Attendances.Count();
            int present = student.Attendances.Count(a => a.Status == "Present");
            var centerValue = total > 0 ? $"{(present * 100 / total)}%" : "0%";


            AttendanceChartList.Add(doughnutChart);






            Chart doughnutChart2 = new Chart();

            doughnutChart2.Type = Enums.ChartType.Doughnut;

            var doughnutData2 = new ChartJSCore.Models.Data();

            doughnutData2.Labels = new List<string>() { "Present" };

            DoughnutDataset doughnutDataset2 = new DoughnutDataset()
            {
                Label = "Attendance Breakdown",
                Data = new List<double?>
    {
        student.Attendances.Count(a => a.Status == "Present" || a.Status == "Late"),
        student.Attendances.Count(a => a.Status == "Absent")
    },
                BackgroundColor = new List<ChartColor>
    {
        ChartColor.FromRgba(0, 163, 108, 0.9),
        ChartColor.FromRgba(216,216,216, 0.8)
    },
                BorderColor = new List<ChartColor>
    {
        ChartColor.FromRgba(255, 255, 255, 1),
        ChartColor.FromRgba(255, 255, 255, 1)
    },
                BorderWidth = new List<int> { 2, 2 },
                HoverOffset = 25
            };

            doughnutData2.Datasets = new List<Dataset>();
            doughnutData2.Datasets.Add(doughnutDataset2);

            doughnutChart2.Data = doughnutData2;

            AttendanceChartList.Add(doughnutChart2);



        }
    }
}
