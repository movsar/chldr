using chldr_shared.Models;
using chldr_test_utils;
using chldr_utils.Services;
using MimeKit;
using Moq;
using System.Reactive.Subjects;

namespace chldr_utils.tests
{
    public class EmailService_MockSend_Success
    {
        [Fact]
        public void Send_NoError_WhenValidMessage()
        {
            // Arrange

            var emailService = TestDataFactory.CreateFakeEmailService();
            var message = new EmailMessage(
                 new string[] { "recipient@example.com" },
                 "Test Email",
                 "<p>Hello, this is a test email!</p>");

            // Act
            emailService.Send(message);

            // Assert

            // Check that the message was added to the SentMessages list
            Assert.Single(emailService.SentMessages);
            Assert.Equal(message, emailService.SentMessages[0]);
        }
    }
}