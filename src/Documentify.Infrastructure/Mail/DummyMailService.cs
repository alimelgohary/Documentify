using Documentify.ApplicationCore.Mail;
using Microsoft.Extensions.Logging;

namespace Documentify.Infrastructure.Mail
{
    public class DummyMailService(ILogger<DummyMailService> _logger) : IMailService
    {
        public Task<bool> SendConfirmMail(string subject, string to, string link, string? from = null)
        {
            _logger.LogInformation("Your link is: {token}", link);
            return Task.FromResult(true);
        }
    }
}
