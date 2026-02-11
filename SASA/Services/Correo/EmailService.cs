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
            _settings = options?.Value ?? new ConfiguracionEmail();

            if (!TieneConfiguracionValida(_settings))
                return;

            var credential = new ClientSecretCredential(_settings.TenantId, _settings.ClientId, _settings.ClientSecret);
            _graphClient = new GraphServiceClient(credential);
        }

        private static bool TieneConfiguracionValida(ConfiguracionEmail s) =>
            !string.IsNullOrWhiteSpace(s.TenantId) &&
            !string.IsNullOrWhiteSpace(s.ClientId) &&
            !string.IsNullOrWhiteSpace(s.ClientSecret) &&
            !string.IsNullOrWhiteSpace(s.FromEmail);



        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            if (_graphClient is null || string.IsNullOrWhiteSpace(_settings.FromEmail))
                return;
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
