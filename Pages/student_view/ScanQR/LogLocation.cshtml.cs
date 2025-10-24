using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttendanceSystem.Pages
{
    [IgnoreAntiforgeryToken] // allows fetch() without form token
    public class LogLocationModel : PageModel
    {
        public void OnGet() { }

        [HttpPost]
        public IActionResult OnPost([FromBody] LocationData data)
        {
            if (data == null)
                return BadRequest();

            // Log to file
            Log.Information("GPS: Lat={Latitude}, Lon={Longitude}, Accuracy={Accuracy}m",
                data.Latitude, data.Longitude, data.Accuracy);

            return new JsonResult(new { message = "Location logged successfully!" });
        }

        public class LocationData
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Accuracy { get; set; }
        }
    }
}
