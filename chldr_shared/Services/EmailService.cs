using chldr_data.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Services
{
    public class EmailService
    {
        private const string UserName = "no-reply@nohchiyn-mott.com";
        private const string Password = "6MsyThgtYWiFTND";
        private const string SmtpServer = "mail.hosting.reg.ru";
        private const int Port = 465;
        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = message.Content;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(UserName, UserName));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = builder.ToMessageBody();
            return emailMessage;
        }
        public void Send(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);

            using var client = new SmtpClient();
            try
            {
                client.Connect(SmtpServer, Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(UserName, Password);
                client.Send(emailMessage);
            }
            catch
            {
                //log an error message or throw an exception or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
