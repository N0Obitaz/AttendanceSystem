using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZXing;
using ZXing.Windows.Compatibility;
using System.Drawing;
using Newtonsoft.Json;
namespace AttendanceSystem.Pages.ScanQR
{
    public class ScanQRModel : PageModel
    {
        [BindProperty]
        public IFormFile InputFile { get; set; }

        [BindProperty]
        public string? OutputText { get; set; }
        public string? OutputEmail { get; set; }
        public string? LastName { get; set; }
        public string? StudentNumber { get; set; }
        // Handle form submission
        public void OnPost()
        {
            OutputText = ScanQRCode(InputFile);

            var jsonData = JsonConvert.DeserializeObject<dynamic>(OutputText);

            OutputEmail = jsonData?.Email;
            LastName = jsonData?.LastName;
            StudentNumber = jsonData?.StudentNumber;
        }

        //decode QR
        public string ScanQRCode(IFormFile file)
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
                return "Error Decoding QR Code" + ex;
            }
        }
    }
}
