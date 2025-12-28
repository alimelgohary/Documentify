namespace Documentify.ApplicationCore.Mail
{
    public interface IMailService
    {
        Task<bool> SendConfirmMail(string subject, string to, string link, string? from = null);
    }
}
