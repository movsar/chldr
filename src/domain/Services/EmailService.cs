using MimeKit;
using domain.Interfaces;
using chldr_utils.Services;
using domain.Models;
using Microsoft.Extensions.Configuration;

namespace domain.Services
{
    public class EmailService
    {
        private Func<ISmtpClientWrapper> _smtpClientFactory;
        // Configuration properties, you can set them during the initialization of the service
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public EmailService(Func<ISmtpClientWrapper> smtpClientFactory)
        {
            _smtpClientFactory = smtpClientFactory;
        }

        public EmailService(IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("Email");

            SmtpServer = emailSettings["SmtpServer"]!;
            Port = int.Parse(emailSettings["Port"]!);
            Username = emailSettings["Username"]!;
            Password = emailSettings["Password"]!;

            _smtpClientFactory = () => new SmtpClientWrapper();
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = message.Content;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Dosham", Username));
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
                client.Authenticate(Username, Password);
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


    }
}
