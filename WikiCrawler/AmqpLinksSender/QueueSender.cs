using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WikiCrawler.AmqpLinksSender
{
    public class QueueSender : IDisposable
    {
        private readonly string _hostName;
        private readonly string _queueName;
        private readonly bool _durable = false;
        private readonly bool _exclusive = false;
        private readonly bool _autoDelete = false;
        private readonly IDictionary<string, object> _arguments = null;

        private readonly IConnection connection;
        private readonly IModel channel;
        private bool _isDisposed;
        ILogger<QueueSender> _log;

        public QueueSender(ILogger<QueueSender> logger, string hostName, string queueName,
                         bool durable, bool exclusive, bool autoDelete,
                         IDictionary<string, object> arguments)
        {
            _hostName = hostName;
            _queueName = queueName;
            _durable = durable;
            _exclusive = exclusive;
            _autoDelete = autoDelete;
            _arguments = arguments;

            _log = logger;

            var factory = new ConnectionFactory() { HostName = _hostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: _durable, exclusive: _exclusive,
                                 autoDelete: _autoDelete, arguments: _arguments);
            
                                 _log.LogInformation($"Connected to Queue: {_queueName}");
        }


        public void SendToQueue(string body)
        {
            var decodeBody = Encoding.UTF8.GetBytes(body);
            channel.BasicPublish(exchange: "", routingKey: _queueName, null, body: decodeBody);

            _log.LogInformation($"Send: {body}");
        }


        public void Dispose()
        {
            if (!_isDisposed)
            {
                // the order is important
                channel.Dispose();
                connection.Dispose();

                _isDisposed = true;

                _log.LogWarning($"Queue {_queueName} Disposed");
            }
        }
    }
}