using ApartmentManagementSystem.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

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
            var client = new SendGridClient(_emailSetting.ApiKey);
    
            try
            {
                var from = new EmailAddress(_emailSetting.Email, "ApartmentManagementSystem");
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}