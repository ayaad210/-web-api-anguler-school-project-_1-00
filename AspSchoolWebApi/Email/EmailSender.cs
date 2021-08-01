using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Email
{
    public class EmailSender : IEmailSender
    {

        public EmailSender()
        {
        }

        public object MailHelper { get; private set; }

        public async Task<SendEmailResponse> SendEmailAsync(string userEmail, string emailSubject, string message)
        {
            var apiKey = "";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("techhowdyblog@gmail.com", "TECHHOWDY.COM");
            var subject = emailSubject;
            var to = new EmailAddress(userEmail, "Test");
            var plainTextContent = message;
            var htmlContent = message;
            SendGridMessage msg = new SendGridMessage();// (from, to, subject, plainTextContent, htmlContent);
            msg.From = from;
            msg.Subject = subject;
            msg.AddTo(to);
            msg.PlainTextContent = plainTextContent;
            msg.HtmlContent = message;


             var response = await client.SendEmailAsync(msg);

            return new SendEmailResponse();
        }
    }
}
