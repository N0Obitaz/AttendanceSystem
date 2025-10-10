
using System.Security.Cryptography;
using System.Text;
using System.Web;
namespace AttendanceSystem.Services
{
    public class TokenService
    {
        private readonly string _secretKey = "SuperSecretKey12345"; // Must be stored Securely

        public string GenerateToken(string email)
        {
            var expiry = DateTime.UtcNow.AddMinutes(10);
            var payload = $"{email}|{expiry:o}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

            var token = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(payload))}.{hash}";
            return HttpUtility.UrlEncode(token);
        }

        public bool ValidateToken(string token, out string email)
        {
            email = string.Empty;
            try
            {
                token = HttpUtility.UrlDecode(token);
                var parts = token.Split('.');

                if (parts.Length != 2)
                    return false;

                var payloadBytes = Convert.FromBase64String(parts[0]);
                var payload = Encoding.UTF8.GetString(payloadBytes);
                var hash = parts[1];

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
                var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

                if (computedHash != hash)
                    return false;

                var payloadParts = payload.Split('|');
                if (payloadParts.Length != 2)
                    return false;

                email = payloadParts[0];
                var expiry = DateTime.Parse(payloadParts[1]);
                return DateTime.UtcNow <= expiry;
            }
            catch
            {
                return false;
            }
        }
    }
}
