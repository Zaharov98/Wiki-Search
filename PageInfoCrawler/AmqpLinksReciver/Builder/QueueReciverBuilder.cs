using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Indexer.AmqpLinksReciver.Builder
{
    public sealed class QueueReciverBuilder
    {
        private string _hostName;
        private string _queueName;

        private bool _durable = false;
        private bool _exclusive = false;
        private bool _autoDelete = false;
        private IDictionary<string, object> _arguments;

        private ILogger<QueueReciver> _log;

        public QueueReciverBuilder() { }


        public QueueReciver Build()
        {
            QueueReciver processor = new QueueReciver(
                logger: _log, hostName: _hostName, queueName: _queueName, durable: _durable,
                exclusive: _exclusive, autoDelete: _autoDelete, arguments: _arguments
            );

            return processor;
        }


        public QueueReciverBuilder HostName(string name)
        {
            _hostName = name;
            return this;
        }

        public QueueReciverBuilder QueueName(string name)
        {
            _queueName = name;
            return this;
        }

        public QueueReciverBuilder Durable(bool durable)
        {
            _durable = durable;
            return this;
        }

        public QueueReciverBuilder Exclusive(bool exclusive)
        {
            _exclusive = exclusive;
            return this;
        }

        public QueueReciverBuilder AutoDelete(bool autoDelete)
        {
            _autoDelete = autoDelete;
            return this;
        }

        public QueueReciverBuilder Arguments(IDictionary<string, object> arguments)
        {
            _arguments = arguments;
            return this;
        }

        public QueueReciverBuilder Logger(ILogger<QueueReciver> logger)
        {
            _log = logger;
            return this;
        }
    }
}
