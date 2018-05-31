using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RmqQueue;

namespace WikiCrawler.AmqpLinksSender
{
    public class QueueSender : RmqQueueBase, IQueueSender<string>, IDisposable
    {
        private readonly ILogger<QueueSender> _log;

        public QueueSender(ILogger<QueueSender> logger, string hostName, string queueName,
                         bool durable, bool exclusive, bool autoDelete,
                         IDictionary<string, object> arguments)
            : base(hostName, queueName, durable, exclusive, autoDelete, arguments)
        {
            _log = logger;
            _log.LogInformation($"Connected to Queue: {_queueName}");
        }


        public void SendToQueue(string body)
        {
            var decodeBody = Encoding.UTF8.GetBytes(body);
            channel.BasicPublish(exchange: "", routingKey: _queueName, null, body: decodeBody);

            _log.LogInformation($"Send: {body}");
        }


        public new void Dispose()
        {
            base.Dispose();
            _log.LogWarning($"Queue {_queueName} Disposed");
        }
    }
}