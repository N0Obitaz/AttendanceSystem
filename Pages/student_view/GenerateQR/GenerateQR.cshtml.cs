using System.Security.Claims;
using AttendanceSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRCoder;
namespace AttendanceSystem.Pages.student_view.GenerateQR
{


    [Authorize]
    public class GenerateQRModel : PageModel
    {
        private readonly AttendanceSystem.Data.AttendanceSystemContext _context;

        [BindProperty]
        public string QRCodeBase64 { get; set; } = string.Empty;
        public GenerateQRModel(AttendanceSystem.Data.AttendanceSystemContext Context)
        {
            _context = Context;
        }
        public Student Student { get; set; }

        [Authorize]
        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                string username = User.Identity.Name;

                Student = await _context.Student
                    .FirstOrDefaultAsync(s => s.StudentId.ToString() == username);



                var JsonContent = new
                {
                    StudentNumber = Student.StudentId,
                    email = Student.Email,
                    lastname = Student.LastName,
                    institute = Student.Institute,
                    Date = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")
                };


                //convert into json using newtonsoft library
                string jsonString = JsonConvert.SerializeObject(JsonContent, Formatting.Indented);



                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(jsonString, QRCodeGenerator.ECCLevel.Q);
                    var qrCode = new PngByteQRCode(qrCodeData);

                    var qrCodeBytes = qrCode.GetGraphic(20);

                    QRCodeBase64 = Convert.ToBase64String(qrCodeBytes);

                    


                    var safeFileName = string.Join("_", "AttendanceQR".Split(Path.GetInvalidFileNameChars()));

                    if (Student == null)
                    {
                        return;
                    }
                }
            }
        }
    }
}
