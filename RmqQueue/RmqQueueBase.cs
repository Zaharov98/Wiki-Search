using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace RmqQueue
{
    public abstract class RmqQueueBase : IDisposable
    {
        protected readonly string _hostName;
        protected readonly string _queueName;
        protected readonly bool _durable = false;
        protected readonly bool _exclusive = false;
        protected readonly bool _autoDelete = false;
        protected readonly IDictionary<string, object> _arguments = null;

        protected readonly IConnection connection;
        protected readonly IModel channel;
        protected bool _isDisposed;

        public RmqQueueBase(string hostName, string queueName,
            bool durable, bool exclusive, bool autoDelete,
            IDictionary<string, object> arguments)
        {
            _hostName = hostName;
            _queueName = queueName;
            _durable = durable;
            _exclusive = exclusive;
            _autoDelete = autoDelete;
            _arguments = arguments;

            var factory = new ConnectionFactory() { HostName = _hostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: _durable, exclusive: _exclusive,
                autoDelete: _autoDelete, arguments: _arguments);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                // the order is important
                channel.Dispose();
                connection.Dispose();

                _isDisposed = true;
            }
        }
    }
}