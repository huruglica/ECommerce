using EmailService.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using static EmailService.EmailService;

namespace EmailService.Services
{
    public class EmailService : EmailServiceBase
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public override async Task<Empty> SendEmail(Request request, ServerCallContext context)
        {
            var name = request.Name;
            var email = request.Email;
            var amount = request.Amount;

            var message = GetMessage(name, amount);

            SendEmail(email, message);

            return await Task.FromResult(new Empty());
        }

        private string GetMessage(string name, double amount)
        {
            var path = @"Templates/EmailTemplate.html";

            StreamReader streamReader = File.OpenText(path);

            string htmlBody = streamReader.ReadToEnd();

            return string.Format(htmlBody, name, amount);
        }

        private void SendEmail(string recieverEmail, string message)
        {
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse(_emailSettings.SenderEmail));
            emailToSend.To.Add(MailboxAddress.Parse(recieverEmail));
            emailToSend.Subject = "ECommerc gift";
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };


            var emailClient = new SmtpClient();
            emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            emailClient.Authenticate(_emailSettings.SenderEmail, _emailSettings.Password);
            emailClient.Send(emailToSend);
            emailClient.Disconnect(true);
        }
    }
}
