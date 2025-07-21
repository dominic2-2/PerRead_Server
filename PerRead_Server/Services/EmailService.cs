using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PerRead_Server.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = _config["Gmail:Username"];
            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException("Gmail:Username configuration is missing or empty.");
            }

            var portString = _config["Gmail:Port"];
            if (string.IsNullOrWhiteSpace(portString))
            {
                throw new InvalidOperationException("Gmail:Port configuration is missing or empty.");
            }

            var enableSslString = _config["Gmail:EnableSsl"];
            if (string.IsNullOrWhiteSpace(enableSslString))
            {
                throw new InvalidOperationException("Gmail:EnableSsl configuration is missing or empty.");
            }

            var smtpClient = new SmtpClient(_config["Gmail:Host"])
            {
                Port = int.Parse(portString),
                Credentials = new NetworkCredential(fromEmail, Environment.GetEnvironmentVariable("SMTP_PASSWORD")),
                EnableSsl = bool.Parse(enableSslString)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, _config["Gmail:SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
