using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using ZXing;
using ZXing.Windows.Compatibility;
using System.Drawing;
using Newtonsoft.Json;
using AttendanceSystem.Services;


namespace AttendanceSystem.Pages.ScanQR
{
    [IgnoreAntiforgeryToken]
    [CustomAuthorize]
    public class ScanQRModel : PageModel
    {
        [BindProperty]
        public IFormFile InputFile { get; set; }

        [BindProperty]
        public string? OutputText { get; set; }
        public string? OutputEmail { get; set; }
        public string? LastName { get; set; }
        public string? StudentNumber { get; set; }

        public string? Name { get; set; }

        [BindProperty]
        public bool IsLocated { get; set; } = false;


        public void OntGet()
        {
            Name = "Charles";
        }


        //  Handler 1: Decode QR Code
        public void OnPostScan()
        {
            OutputText = ScanQRCode(InputFile);

            try
            {
                var jsonData = JsonConvert.DeserializeObject<dynamic>(OutputText);
                OutputEmail = jsonData?.Email;
                LastName = jsonData?.LastName;
                StudentNumber = jsonData?.StudentNumber;
            }
            catch (Exception ex)
            {
                OutputEmail = "Error parsing JSON: " + ex.Message;
                LastName = "Error parsing JSON: " + ex.Message;
                StudentNumber = "Error parsing JSON: " + ex.Message;
            }
        }

        // Handler 2: Log GPS Location (called via JS fetch)

        public IActionResult OnPostLogLocation([FromBody] LocationData data)
        {
            if (data == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid request — no location data received."
                })
                {
                    StatusCode = 400
                };
            }
            TempData["Message"] = $"Received Location";
            var locationService = new LocationService();
            var validation = locationService.ValidateLocation(data.Latitude, data.Longitude, data.Accuracy);

            //TempData["isValid"] = validation.IsWithinPerimeter;
            IsLocated = validation.IsWithinPerimeter;
            TempData["isValid"] = IsLocated;
            if (validation.IsWithinPerimeter)
            {
                Log.Information("Location Validated: {Message}", validation.Message);
                return new JsonResult(new
                {
                    success = true,
                    message = validation.Message,
                    distance = validation.Distance
                });
                
            }
            else
            {
                Log.Warning("Location INVALID: {Message}", validation.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = validation.Message,
                    distance = validation.Distance
                });
            }
        }


        // Helper: QR Code Decoding
        private string ScanQRCode(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return "No File Detected";

            try
            {
                using var stream = file.OpenReadStream();
                using var bitmap = new Bitmap(stream);

                var reader = new BarcodeReaderGeneric();
                var result = reader.Decode(new BitmapLuminanceSource(bitmap));

                return result?.Text ?? "No QR Code Detected";
            }
            catch (Exception ex)
            {
                return "Error Decoding QR Code: " + ex.Message;
            }
        }

        

        // Location data class
        public class LocationData
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Accuracy { get; set; }
        }
    }
}
