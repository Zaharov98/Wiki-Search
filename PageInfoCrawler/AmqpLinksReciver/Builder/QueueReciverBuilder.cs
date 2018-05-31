using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using RmqQueue;
using RmqQueue.Builder;

namespace PageInfoCrawler.AmqpLinksReciver.Builder
{
    public sealed class QueueReciverBuilder : RmqQueueBuilder<QueueReciver>
    {
        private ILogger<QueueReciver> _logger;

        public QueueReciverBuilder() { }

        public override QueueReciver Build()
        {
            QueueReciver processor = new QueueReciver(
                logger: _logger, hostName: base.hostName, queueName: base.queueName, durable: base.durable, 
                exclusive: base.exclusive, autoDelete: base.autoDelete, arguments: base.arguments
            );

            return processor;
        }

        public QueueReciverBuilder Logger(ILogger<QueueReciver> logger)
        {
            this._logger = logger;
            return this;
        }
        
    }
}
