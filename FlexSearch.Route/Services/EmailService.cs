using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FlexSearch.Router.Models;

namespace FlexSearch.Router.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISettingsService _settingsService;

        public EmailService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<bool> SendEmailNotification(string emailText)
        {
            var settings = await _settingsService.GetSettings();

            var from = new MailAddress(settings.EmailData.EmailLogin, settings.EmailData.EmailLogin);
            var subject = "FLEX SEARCH";

            var client = new SmtpClient(settings.EmailData.EmailHost, Convert.ToInt32(settings.EmailData.EmailPort));
            client.Credentials = new NetworkCredential(settings.EmailData.EmailLogin, settings.EmailData.EmailPassword);
            client.EnableSsl = settings.EmailData.EnableSsl;

            Parallel.ForEach(settings.EmailNotifications.Addresses, async address =>
            {
                var to = new MailAddress(address);

                var message = new MailMessage(from, to)
                {
                    Body = emailText,
                    Subject = subject
                };

                await client.SendMailAsync(message);
            });

            return true; 
        }
    }
}