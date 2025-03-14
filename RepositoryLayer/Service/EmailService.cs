using System;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using RepositoryLayer.Helper;
using Microsoft.Extensions.Configuration;

namespace RepositoryLayer.Service
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IConfiguration _configuration;

        public EmailService(IOptions<SmtpSettings> smtpSettings, IConfiguration configuration)
        {
            _smtpSettings = smtpSettings.Value;
            _configuration = configuration; 
        }

        public bool SendPasswordResetEmail(string email, string token, string baseUrl)
        {
            try
            {
                if (_smtpSettings == null)
                {
                    Console.WriteLine("SMTP settings are not initialized.");
                    return false;
                }
                if (string.IsNullOrEmpty(_smtpSettings.Server) ||
                    string.IsNullOrEmpty(_smtpSettings.SenderEmail) ||
                    string.IsNullOrEmpty(_smtpSettings.Username) ||
                    string.IsNullOrEmpty(_smtpSettings.Password))
                {
                    Console.WriteLine("One or more SMTP settings are missing.");
                    return false;
                }

                var resetLink = $"{baseUrl}/api/AddressBookApplication/reset-password?token={token}";
                Console.WriteLine($"Generated Reset Link: {resetLink}");

                var mail = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                    Subject = "Password Reset Request",
                    Body = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hello,</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This link will expire in 1 hour.</p>
                </body>
                </html>",
                    IsBodyHtml = true
                };

                mail.To.Add(email);

                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.Send(mail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        public async Task SendWelcomeEmailAsync(string email)
        {
            Console.WriteLine($"[Debug] Sending email to: {email}");

            if (!IsValidEmail(email))
            {
                Console.WriteLine($"[Error] Invalid email format: {email}");
                return;
            }

            using (var smtpClient = new SmtpClient(_configuration["Smtp:Server"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Smtp:SenderEmail"]),
                    Subject = "Welcome to AddressBook",
                    Body = $"Hello,\n\nThank you for registering with AddressBook!\n\nBest Regards,\nTeam",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[RabbitMQ] Welcome email sent to {email}");
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }


    }
}
