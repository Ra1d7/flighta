using static Flighta.Helpers.EmailSender;

namespace Flighta.Helpers
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(EmailData data);
    }
}
