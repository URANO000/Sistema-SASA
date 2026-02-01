using System.Net;
using System.Net.Mail;
using BusinessLogic.Servicios.Correo;
using Microsoft.Extensions.Options;

namespace SASA.Services.Correo
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpEmailSettings _opt;

        public SmtpEmailSender(IOptions<SmtpEmailSettings> opt)
        {
            _opt = opt.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            using var msg = new MailMessage
            {
                From = new MailAddress(_opt.FromEmail, _opt.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            msg.To.Add(toEmail);

            using var client = new SmtpClient(_opt.Host, _opt.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_opt.User, _opt.Pass)
            };

            await client.SendMailAsync(msg);
        }
    }
}
