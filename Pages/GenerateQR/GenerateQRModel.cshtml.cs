using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using AttendanceSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using QRCoder;
using ZXing;
using ZXing.Windows.Compatibility;
using ZXing.Common;
using ZXing.QrCode;



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


        //create a bind property for file upload
        [BindProperty]
        public IFormFile InputFile { get; set; }

        // Create a property to hold the generated QR code image as a base64 string
        [BindProperty]
        public string? OutputText{ get; set; } 

        [BindProperty]
        public string? OutputEmail { get; set; }

        [BindProperty]
        public string? OutputLastName { get; set; }



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

            // Decode the QR code to verify its content (for demonstration purposes)
            // what library to use to decode the qr code
            // using (var ms = new MemoryStream(Convert.FromBase64String(QRCodeBase64)))

           // decode the submitted qr code png file into json using qrcode library
           // use zxing.net library to decode the qr code png file
           

        }
       



        }
    }

