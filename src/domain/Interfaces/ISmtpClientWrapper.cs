using MimeKit;

namespace domain.Interfaces
{
    public interface ISmtpClientWrapper : IDisposable
    {
        void Connect(string host, int port, bool useSsl);
        void Authenticate(string userName, string password);
        void Send(MimeMessage message);
        void Disconnect(bool quit);
    }
}
