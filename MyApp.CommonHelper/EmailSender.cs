using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.CommonHelper
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string From = "personal.project.work.area@gmail.com";
            string Password = "uqewemmsudhosmyg";

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(From);
            mailMessage.Subject = subject;
            mailMessage.To.Add(new MailAddress("rathodharsh1974@gmail.com"));
            mailMessage.Body = htmlMessage;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(From, Password),
                EnableSsl = true
            };

            smtpClient.Send(mailMessage);
            return Task.CompletedTask;
        }
    }
}
