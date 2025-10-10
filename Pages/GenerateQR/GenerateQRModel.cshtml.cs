using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;

using System.Net.Mail;

namespace AttendanceSystem.Pages.GenerateQR
{
    public class GenerateQRModel : PageModel
    {

        [BindProperty]
        public string InputText { get; set; } = string.Empty;

        [BindProperty]
        public string InputEmail { get; set; } = string.Empty;
        [BindProperty]
        public string LastName { get; set; } = string.Empty;
        [BindProperty]
        public string FileName { get; set; } = string.Empty;

        public string QRCodeBase64 { get; set; } = string.Empty;

        public void OnPost()
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




            }
        }
    }
}
