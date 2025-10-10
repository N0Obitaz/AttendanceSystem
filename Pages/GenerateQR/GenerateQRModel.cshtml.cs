using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;
using AttendanceSystem.Services;
using System.Threading.Tasks;


namespace AttendanceSystem.Pages.GenerateQR
{
    public class GenerateQRModel : PageModel
    {

        private readonly EmailService _emailService;

        public GenerateQRModel(EmailService emailService)
        {
            _emailService = emailService;
        }

        [BindProperty]
        public string InputText { get; set; } = string.Empty;

        [BindProperty]
        public string InputEmail { get; set; } = string.Empty;
        [BindProperty]
        public string LastName { get; set; } = string.Empty;
        [BindProperty]
        public string FileName { get; set; } = string.Empty;

        public string QRCodeBase64 { get; set; } = string.Empty;

        [TempData]
        public string Linked { get; set; }
        public async Task OnPost()
        {
            if (string.IsNullOrWhiteSpace(InputText))
                return;

            var JsonContent = new
            {
                StudentNumber = InputText,
                Email = InputEmail,
                LastName = LastName,
                Date = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")
            };

            //convert into json using newtonsoft library
            string jsonString = JsonConvert.SerializeObject(JsonContent, Formatting.Indented);


            var tokenService = new TokenService();
            var token = tokenService.GenerateToken(InputEmail);
            
            var link = $"https://localhost:7232/Attendances/StudentMarkAttendanceModel?handler=Record&token={token}&id={InputText}&status=Present";


            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(jsonString, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);

                var qrCodeBytes = qrCode.GetGraphic(20);

                QRCodeBase64 = Convert.ToBase64String(qrCodeBytes);

                // Optionally, save the QR code as an image file on the a certain image folder

                var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

         
                if (!Directory.Exists(wwwRootPath))
                {
                    Directory.CreateDirectory(wwwRootPath);
                }

              
                var safeFileName = string.Join("_", FileName.Split(Path.GetInvalidFileNameChars()));

                var imagePath = Path.Combine(wwwRootPath, $"{safeFileName}.png");

               

                System.IO.File.WriteAllBytes(imagePath, qrCodeBytes);

                Linked = link;
                string subject = "Your Attendance Link";
                string body = $"<p>Dear Student,</p><p>Below is your Attendance Link.</p>" +
                    $"<a href={link}>Click Here To Mark Your Attendance </a><hr/ ><p></p><p>Best regards,<br/>Attendance System</p>";

                await _emailService.SendEmailAsync(InputEmail, subject, body);
            }

          

        }
    }
}
