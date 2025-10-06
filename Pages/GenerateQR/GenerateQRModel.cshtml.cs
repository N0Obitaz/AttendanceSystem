using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AttendanceSystem.Pages.GenerateQR
{
    public class GenerateQRModel : PageModel
    {

        [BindProperty]
        public string InputText { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string QRCodeBase64 { get; set; } = string.Empty;

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(InputText))
                return;

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(InputText, QRCodeGenerator.ECCLevel.Q);
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
