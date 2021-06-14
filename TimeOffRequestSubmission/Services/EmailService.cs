using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public class EmailService: IEmailService
    {
        private readonly EmailSettings _mailSettings;

        public EmailService(IOptions<EmailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        
        // send email
        public async Task SendAsync(EmailRequest emailRequest)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(emailRequest.From),
                To = {MailboxAddress.Parse(emailRequest.To)},
                Subject = emailRequest.Subject
            };
            var body = new BodyBuilder {HtmlBody = emailRequest.Body};
            email.Body = body.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}