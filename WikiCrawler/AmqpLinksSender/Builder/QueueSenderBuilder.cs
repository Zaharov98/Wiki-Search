using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RmqQueue;
using RmqQueue.Builder;
using WikiCrawler.AmqpLinksSender;

namespace WikiCrawler.AmqpLinksSender.Builder
{
    public sealed class QueueSenderBuilder : RmqQueueBuilder<QueueSender>
    {
        private ILogger<QueueSender> _logger;    

        public override QueueSender Build()
        {
            QueueSender processor = new QueueSender(
                logger: _logger, hostName: base.hostName, queueName: base.queueName, durable: base.durable, 
                exclusive: base.exclusive, autoDelete: base.autoDelete, arguments: base.arguments
            );

            return processor;
        }

        public RmqQueueBuilder<QueueSender> Logger(ILogger<QueueSender> logger)
        {
            this._logger = logger;
            return this;
        }
    }
}