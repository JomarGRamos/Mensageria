using Domain.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mensageria
{
    public class Worker : BackgroundService
    {
        private readonly IMessageQueue _messageQueue;
        private readonly int idService = DateTime.Now.Millisecond;

        public Worker(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageQueue.Consumer(idService);

            while (!stoppingToken.IsCancellationRequested)
            {
                await _messageQueue.Publisher(idService);
                await Task.Delay(5000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

    }
}
