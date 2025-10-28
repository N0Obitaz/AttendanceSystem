using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AttendanceSystem.Data;
using AttendanceSystem.Models;
using AttendanceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
//using System.Drawing;
using ZXing;
using ZXing.ImageSharp;
//using ZXing.Windows.Compatibility;

namespace AttendanceSystem.Pages.ScanQR
{
    [IgnoreAntiforgeryToken]
    [Authorize]
    public class ScanQRModel : PageModel
    {
        private readonly AttendanceSystemContext _context;

        public ScanQRModel(AttendanceSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IFormFile? InputFile { get; set; }

        [BindProperty]
        public string? OutputText { get; set; }

        public string? OutputEmail { get; set; }
        public string? LastName { get; set; }
        public string? StudentNumber { get; set; }
        public string? Name { get; set; }

        [BindProperty]
        public bool IsLocated { get; set; } = false;

        public bool IsValidQR { get; set; } = false;


        public void OnGet()
        {

            Name = "Charles";
        }

        

        //
        //   Log Location
        // 
        public async Task<IActionResult> OnPostLogLocation([FromBody] LocationData? data)
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

            var locationService = new LocationService();
            var validation = locationService.ValidateLocation(data.Latitude, data.Longitude, data.Accuracy);
            IsLocated = validation.IsWithinPerimeter;

            if (!validation.IsWithinPerimeter)
            {
                Log.Warning("Invalid location: {Message}", validation.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = validation.Message,
                    distance = validation.Distance
                });
            }

            Log.Information("Location Validated: {Message}", validation.Message);

            // Retrieve saved QR details
            var studentNumber = TempData["StudentNumber"] as string;
            bool isValidQR = TempData["IsValidQR"] != null && (bool)TempData["IsValidQR"];

            if (!isValidQR)
            {
                return new JsonResult(new { success = false, message = "QR code not validated." });
            }

            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                return new JsonResult(new { success = false, message = "Student number not found." });
            }

            // Clean & mark attendance
            string cleanStudentNumber = new string(studentNumber.Where(s => s != '-').ToArray());
            if (int.TryParse(cleanStudentNumber, out int studentId))
            {
                try
                {
                    await MarkAttendance(studentId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = $"{validation.Message}\n Checking Attendance...",
                        distance = validation.Distance
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error marking attendance for Student ID {StudentId}", studentId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Error marking attendance. Please contact admin."
                    });
                }
            }

            return new JsonResult(new { success = false, message = "Invalid student number format." });
        }

        //  Mark Attendance 
  
        private async Task MarkAttendance(int id)
        {
            var student = await _context.Student
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            bool alreadyMarked = await _context.Attendances
                .AnyAsync(a => a.StudentId == id && a.Date.Date == DateTime.UtcNow.Date);

            if (student == null)
            {

                Log.Warning("Student not found: ID {Id}", id);
                throw new Exception("Student not found.");
            
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            if (alreadyMarked)
            {
                Log.Warning("Attendance already marked for Student ID {StudentId}", id);
                throw new Exception("Attendance already marked for today.");
            }
            try
            {
                // Create attendance record
                var attendance = new Attendance
                {
                    StudentId = student.StudentId,
                    Date = DateTime.UtcNow,
                    Status = "Present"
                };

                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Attendance Marked Successfully!";

                // Create transaction log
                var transactionLog = new TransactionLog
                {
                    StudentId = student.StudentId,
                    Action = "Marked Present",
                    Timestamp = DateTime.UtcNow
                };

                _context.TransactionLogs.Add(transactionLog);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                Log.Information("Attendance marked for Student ID {StudentId}", id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // ======================
        //  Handler: Decode QR
        // ======================
        public void  OnPostScan()
        {
            var sampleText = new
            {
                StudentNumber = "23-00185",
                Email = "charlesbernard.balaguer.041504@gmail.com",
                LastName = "Balaguer",
                Date = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")

            };

          



            //OutputText = JsonConvert.SerializeObject(ScanQRCode(InputFile));
            OutputText = ScanQRCode(InputFile); 

            try
            {
                var jsonData = JsonConvert.DeserializeObject<dynamic>(OutputText);
                OutputEmail = jsonData?.Email;
                LastName = jsonData?.LastName;
                StudentNumber = jsonData?.StudentNumber;
                

                // Save to TempData for later use in LogLocation
                if (!string.IsNullOrEmpty(StudentNumber) && jsonData != null)
                {
                    TempData["StudentNumber"] = StudentNumber;
                    TempData["IsValidQR"] = true;
                    Log.Information("QR Scanned Successfully for StudentNumber: {StudentNumber}", StudentNumber);
                }
                else
                {
                    Log.Warning("QR data missing StudentNumber.");
                }
            }
            catch (Exception ex)
            {
                OutputEmail = "Error parsing JSON: " + ex.Message;
                LastName = OutputEmail;
                StudentNumber = OutputEmail;
                Log.Error(ex, "Error decoding QR content");
            }
        }

        private string ScanQRCode(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return "No File Detected";

            try
            {
                //using var stream = file.OpenReadStream();
                //using var bitmap = new Bitmap(stream);

                //var reader = new BarcodeReaderGeneric();
                //var result = reader.Decode(new BitmapLuminanceSource(bitmap));

                //if (result != null)
                //{
                //    IsValidQR = true;
                //    return result.Text;
                //}

                using var image = Image.Load<Rgba32>(file.OpenReadStream());


                // Copy all pixel data into a byte array
                byte[] pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];
                image.CopyPixelDataTo(pixelBytes);


                var luminance = new RGBLuminanceSource(
                    pixelBytes, 
                    image.Width, 
                    image.Height, 
                    RGBLuminanceSource.BitmapFormat.RGB32
                    );




               

                var reader = new BarcodeReaderGeneric();
                var result = reader.Decode(luminance);

                if (result.Text != null)
                {
                    IsValidQR = true;
                    var decodedText = result.Text;
                    Log.Information("Decoded Text" + decodedText);

                    TempData["resultText"] = decodedText;
                    return decodedText;
                }

                TempData["resultText"] = "No QR Code Detected";
                return "No QR Code Detected";
            }
            catch (Exception ex)
            {
                Log.Error("Error decoding QR code" + ex.Message);
                return "Error Decoding QR Code: " + ex.Message;
            }
        }

    
        public class LocationData
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Accuracy { get; set; }
        }
    }
}
