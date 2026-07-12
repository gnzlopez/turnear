using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppointmentManager.Services.Email
{
    public class EmailQueueHostedService : BackgroundService
    {
        private readonly IEmailQueue _queue;
        private readonly IEmailSender _sender;
        private readonly ILogger<EmailQueueHostedService> _logger;

        public EmailQueueHostedService(IEmailQueue queue, IEmailSender sender, ILogger<EmailQueueHostedService> logger)
        {
            _queue = queue;
            _sender = sender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailQueueHostedService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var message = await _queue.DequeueAsync(stoppingToken);
                    if (message is null) continue;

                    await _sender.SendEmailAsync(message.To, message.Subject, message.Body);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending queued email.");
                    // opcional: re-enqueue o manejar reintentos
                }
            }

            _logger.LogInformation("EmailQueueHostedService stopped.");
        }
    }
}