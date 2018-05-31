using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RmqQueue;

namespace PageInfoCrawler.AmqpLinksReciver
{
    public class QueueReciver : RmqQueueBase, IQueueReciver, IDisposable
    {    
        private readonly ILogger<QueueReciver> _log;
        private Subject<string> _messageSubject;


        public QueueReciver(ILogger<QueueReciver> logger, string hostName, string queueName,
                         bool durable, bool exclusive, bool autoDelete,
                         IDictionary<string, object> arguments)
            : base(hostName, queueName, durable, exclusive, autoDelete, arguments)
        {
            _log = logger;
            _messageSubject = new Subject<string>();
            _log.LogInformation($"Connected to Queue: {base._queueName}");
        }


        public new void Dispose()
        {
            base.Dispose();
            _log.LogWarning($"Queue {base._queueName} Disposed");
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
