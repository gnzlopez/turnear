using System.Threading;
using System.Threading.Tasks;

namespace AppointmentManager.Services.Email
{
    public interface IEmailQueue
    {
        void Enqueue(EmailMessage message);
        Task<EmailMessage?> DequeueAsync(CancellationToken cancellationToken);
    }
}