using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentManager.Services.Email
{
    public class BackgroundEmailQueue : IEmailQueue
    {
        private readonly ConcurrentQueue<EmailMessage> _items = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void Enqueue(EmailMessage message)
        {
            _items.Enqueue(message);
            _signal.Release();
        }

        public async Task<EmailMessage?> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _items.TryDequeue(out var message);
            return message;
        }
    }
}