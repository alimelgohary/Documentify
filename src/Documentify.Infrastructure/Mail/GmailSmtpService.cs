using Documentify.ApplicationCore.Mail;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Mail
{
    public class GmailSmtpService(IMemoryCache _cache,
                                  IConfiguration _config,
                                  ILogger<GmailSmtpService> _logger,
                                  UserManager<ApplicationUser> _userManager,
                                  IHostEnvironment _environment
                                    ) : IMailService
    {
        public async Task<bool> SendConfirmMail(string subject, string to, string link, string? from = null)
        {
            
            string key = "email-confirm.html";
            string html = await GetHtmlAsString(key);
            html = string.Format(html, link);  // TODO Enhance this

            await SendMail(subject, html, to);
            return true;
        }
        private async Task<string> GetHtmlAsString(string htmlFile)
        {
            string html;
            if (!_cache.TryGetValue(htmlFile, out html!))
            {
                var assembly = typeof(GmailSmtpService).Assembly;
                using var stream = assembly.GetManifestResourceStream($"Documentify.Infrastructure.Mail.Templates.{htmlFile}");
                if (stream is null)
                    throw new Exception("Mail template not found or not copied to output directory");
                using var reader = new StreamReader(stream);
                html = await reader.ReadToEndAsync();
                _cache.Set(htmlFile, html, TimeSpan.FromMinutes(10));
            }
            return html;
        }
        private async Task SendMail(string subject, string html, string to)
        {
            // Gmail SMTP server settings
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var enableSsl = true;

            // Gmail login credentials
            var gmailUsername = _config[ConfigurationKeys.GmailUser]!;
            var gmailPassword = _config[ConfigurationKeys.GmailPass]!;

            // Email message settings
            var from = gmailUsername; 
            var body = html;

            // Create the email message
            MailMessage mail = new MailMessage(from, to, subject, body);
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.SubjectEncoding = Encoding.UTF8;

            // Set up the SMTP client
            SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
            smtp.EnableSsl = enableSsl;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(gmailUsername, gmailPassword);

            await smtp.SendMailAsync(mail);
        }
    }
}
