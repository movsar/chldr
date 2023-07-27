using System;
using System.Collections.Generic;
using MimeKit;
using MailKit.Net.Smtp;
using chldr_shared.Models;

namespace chldr_utils.Services
{
    public interface ISmtpClientWrapper : IDisposable
    {
        void Connect(string host, int port, bool useSsl);
        void Authenticate(string userName, string password);
        void Send(MimeMessage message);
        void Disconnect(bool quit);
    }

    public class SmtpClientWrapper : ISmtpClientWrapper
    {
        private readonly SmtpClient _smtpClient;

        public SmtpClientWrapper()
        {
            _smtpClient = new SmtpClient();
        }

        public void Connect(string host, int port, bool useSsl)
        {
            _smtpClient.Connect(host, port, useSsl);
        }

        public void Authenticate(string userName, string password)
        {
            _smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
            _smtpClient.Authenticate(userName, password);
        }

        public void Send(MimeMessage message)
        {
            _smtpClient.Send(message);
        }

        public void Disconnect(bool quit)
        {
            _smtpClient.Disconnect(quit);
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }
    }

    public class EmailService
    {
        private readonly Func<ISmtpClientWrapper> _smtpClientFactory;

        public EmailService(Func<ISmtpClientWrapper> smtpClientFactory)
        {
            _smtpClientFactory = smtpClientFactory;
        }

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

        public List<EmailMessage> SentMessages { get; set; } = new List<EmailMessage>();

        public void Send(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);

            using var client = _smtpClientFactory();
            try
            {
                client.Connect(SmtpServer, Port, true);
                client.Authenticate(UserName, Password);
                client.Send(emailMessage);

                SentMessages.Add(message);
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

        // Configuration properties, you can set them during the initialization of the service
        public string SmtpServer { get; set; } = "mail.hosting.reg.ru";
        public int Port { get; set; } = 465;
        public string UserName { get; set; } = "no-reply@nohchiyn-mott.com";
        public string Password { get; set; } = "6MsyThgtYWiFTND";
    }
}
