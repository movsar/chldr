using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using domain.Interfaces;

namespace domain.Services
{

    public class SmtpClientWrapper : ISmtpClientWrapper
    {
        private readonly SmtpClient _smtpClient;

        public SmtpClientWrapper()
        {
            _smtpClient = new SmtpClient();
        }

        public void Connect(string host, int port, bool useSsl)
        {
            _smtpClient.Connect(host, port, SecureSocketOptions.StartTls);
        }

        public void Authenticate(string userName, string password)
        {
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

}
