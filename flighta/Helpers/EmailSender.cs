using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace Flighta.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;
        private string smtpServer = string.Empty;
        private string smtpPort = string.Empty;
        private string smtpEmail = string.Empty;
        private string smtpPassword = string.Empty;

        public EmailSender(IConfiguration config,ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }
        public record EmailData(string fullname, string email, string? phonenumber, string message);
        public async Task<bool> SendEmailAsync(EmailData data)
        {
            smtpServer = _config.GetValue<string>("SmtpData:SmtpServer");
            smtpPort = _config.GetValue<string>("SmtpData:SmtpPort");
            smtpEmail = _config.GetValue<string>("SmtpData:Email");
            smtpPassword = _config.GetValue<string>("SmtpData:SmtpServer");
            try
            {

                var client = new SmtpClient("smtp.office365.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("your.email@live.com", "your password")
                };
                await client.SendMailAsync(
                    new MailMessage(from: "your.email@live.com", //hard coding company email so feedback goes to it (might be bad practice)...not sure
                                    to: "your.email@live.com",
                                    $"Feedback from {data.fullname}",
                                    $"Message From: {data.fullname}\n Sender Email: {data.email}\n Sender Phonenumber: {data.phonenumber}\n\n\n {data.message}"
                                    ));
                _logger.LogDebug($"EmailSender has successfully processed a request for {JsonConvert.SerializeObject(data)} at {DateTime.Now}");
                return true;
            }
            catch
            {
                _logger.LogError($"EmailSender has encountered an error at {DateTime.Now}" , data);
                return false;
            }
        }



    }
}
