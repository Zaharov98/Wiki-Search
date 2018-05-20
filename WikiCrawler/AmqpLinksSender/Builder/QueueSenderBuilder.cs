using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WikiCrawler.AmqpLinksSender.Builder
{
    public sealed class QueueSenderBuilder
    {
        private string _hostName;
        private string _queueName;

        private bool _durable = false;
        private bool _exclusive = false;
        private bool _autoDelete = false;
        private IDictionary<string, object> _arguments;

        private ILogger<QueueSender> _log;

        public QueueSenderBuilder() { }


        public QueueSender Build()
        {
            QueueSender processor = new QueueSender(
                logger: _log, _hostName, _queueName, durable: _durable, exclusive: _exclusive,
                autoDelete: _autoDelete, arguments: _arguments
            );

            return processor;
        }


        public QueueSenderBuilder HostName(string name)
        {
            _hostName = name;
            return this;
        }

        public QueueSenderBuilder QueueName(string name)
        {
            _queueName = name;
            return this;
        }

        public QueueSenderBuilder Durable(bool durable)
        {
            _durable = durable;
            return this;
        }

        public QueueSenderBuilder Exclusive(bool exclusive)
        {
            _exclusive = exclusive;
            return this;
        }

        public QueueSenderBuilder AutoDelete(bool autoDelete)
        {
            _autoDelete = autoDelete;
            return this;
        }

        public QueueSenderBuilder Arguments(IDictionary<string, object> arguments)
        {
            _arguments = arguments;
            return this;
        }

        public QueueSenderBuilder Logger(ILogger<QueueSender> logger)
        {
            _log = logger;
            return this;
        }
    }
}