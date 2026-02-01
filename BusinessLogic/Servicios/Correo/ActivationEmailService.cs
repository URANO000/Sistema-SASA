namespace BusinessLogic.Servicios.Correo
{
    public class ActivationEmailService
    {
        private readonly IEmailSender _email;

        public ActivationEmailService(IEmailSender email)
        {
            _email = email;
        }

        public Task SendActivationAsync(string toEmail, string activationLink)
        {
            var subject = "Activa tu cuenta - SASA";
            var body = $"""
                <p>Hola,</p>
                <p>Para activar tu cuenta, haz clic aquí:</p>
                <p><a href="{activationLink}">Activar cuenta</a></p>
                <p>Si no solicitaste esto, ignora este mensaje.</p>
            """;

            return _email.SendAsync(toEmail, subject, body);
        }
    }
}
