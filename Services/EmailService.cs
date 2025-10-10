
using Microsoft.Extensions.Options;
using System.Net.Mail;
using AttendanceSystem.Models;
using System.Net;


namespace AttendanceSystem.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings) 
        {
            _settings = settings.Value;
            Console.WriteLine($"[DEBUG] Host: {_settings.Host}, Username: {_settings.Username}");
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(_settings.Username))
                throw new InvalidOperationException("Email username not configured properly in appsettings.json.");

            using var message = new MailMessage();
            message.From = new MailAddress(_settings.Username);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            await smtp.SendMailAsync(message);
        }
    }
}
