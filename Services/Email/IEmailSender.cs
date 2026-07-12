using System.Threading.Tasks;

namespace AppointmentManager.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}