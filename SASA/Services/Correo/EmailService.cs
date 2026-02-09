using Azure.Identity;
using BusinessLogic.Servicios.Correo;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;

namespace SASA.Services.Correo
{
    public sealed class EmailService : IEmailService
    {
        private readonly ConfiguracionEmail _settings;
        private readonly GraphServiceClient _graphClient;

        public EmailService(IOptions<ConfiguracionEmail> options)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(_settings.TenantId)) throw new ArgumentException("TenantId es requerido");
            if (string.IsNullOrWhiteSpace(_settings.ClientId)) throw new ArgumentException("ClientId es requerido");
            if (string.IsNullOrWhiteSpace(_settings.ClientSecret)) throw new ArgumentException("ClientSecret es requerido");
            if (string.IsNullOrWhiteSpace(_settings.FromEmail)) throw new ArgumentException("FromEmail es requerido");

            var credential = new ClientSecretCredential(_settings.TenantId, _settings.ClientId, _settings.ClientSecret);
            _graphClient = new GraphServiceClient(credential);
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = htmlBody
                },
                ToRecipients =
                [
                    new Recipient
        {
            EmailAddress = new EmailAddress
            {
                Address = toEmail,
                Name = toName
            }
        }
                ],

                // 🔴 CC TEMPORAL PARA TEST
                CcRecipients =
                [
                    new Recipient
        {
            EmailAddress = new EmailAddress
            {
                Address = "marifermatah@gmail.com",
                Name = "Copia Prueba SASA"
            }
        }
                ]
            };

            try
            {
                await _graphClient.Users[_settings.FromEmail]
                    .SendMail
                    .PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                    {
                        Message = message,
                        SaveToSentItems = true
                    });
            }
            catch (ApiException ex)
            {
                throw new InvalidOperationException(
                    $"Graph ApiException: Status {(int?)ex.ResponseStatusCode} - {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error enviando correo (Graph). {ex.GetType().Name} - {ex.Message}", ex);
            }


        }
    }
}
