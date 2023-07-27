using MimeKit;
using chldr_shared.Models;
using chldr_utils.Interfaces;

namespace chldr_utils.Services
{

    public class EmailService
    {
        private Func<ISmtpClientWrapper> _smtpClientFactory;

        public EmailService(Func<ISmtpClientWrapper> smtpClientFactory)
        {
            _smtpClientFactory = smtpClientFactory;
        }

        public EmailService()
        {
            _smtpClientFactory = () => new SmtpClientWrapper();
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
