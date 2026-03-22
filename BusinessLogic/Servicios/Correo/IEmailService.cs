namespace BusinessLogic.Servicios.Correo
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
    }
}
