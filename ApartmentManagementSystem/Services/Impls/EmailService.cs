using ApartmentManagementSystem.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ApartmentManagementSystem.Services.Impls
{
    class EmailService : IEmailService
    {
        private readonly EmailSetting _emailSetting;

        public EmailService(EmailSetting emailSetting)
        {
            _emailSetting = emailSetting;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("ApartmentManagementSystem", _emailSetting.Email));
            email.To.Add(new MailboxAddress("", toEmail));

            email.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            email.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSetting.Email, _emailSetting.Password);

                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
        }
    }
}