using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PageInfoCrawler.AmqpLinksReciver
{
    public class QueueReciver : IDisposable
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
        ILogger<QueueReciver> _log;

        private Subject<string> _messageSubject;


        public QueueReciver(ILogger<QueueReciver> logger, string hostName, string queueName,
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

            _messageSubject = new Subject<string>();

            var factory = new ConnectionFactory() { HostName = _hostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: _durable, exclusive: _exclusive,
                                 autoDelete: _autoDelete, arguments: _arguments);

            _log.LogInformation($"Connected to Queue: {_queueName}");
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

        public void Subscribe(Action<string> onNext, Action onCompleted)
        {
            _messageSubject.Subscribe(onNext, onCompleted);
        }


        public void StartReciving()
        {
            try
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        _messageSubject.OnNext(message);
                    };

                channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
                _BlockAndListen();  // blockin operation
            }
            catch (Exception e)
            {
                _log.LogError($"Exception occured while receving: {e}");
                throw new OperationCanceledException("Exception while reciving RabbitMQ", e);
            }
        }

        public void _BlockAndListen()
        {
            _log.LogInformation($"Start listening the queue {_queueName}, main thread blocked");
            while (true)
            {
                // Waiting for events from the queue
            }
        }
    }
}
