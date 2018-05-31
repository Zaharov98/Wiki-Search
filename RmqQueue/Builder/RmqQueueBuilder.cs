using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RmqQueue.Builder
{
    public abstract class RmqQueueBuilder<T> where T: RmqQueueBase 
    {
        protected string hostName { private set; get; }
        protected string queueName { private set; get; }

        protected bool durable { private set; get; } = false;
        protected bool exclusive { private set; get; } = false;
        protected bool autoDelete { private set; get; } = false;
        protected IDictionary<string, object> arguments { private set; get; }

        public abstract T Build();


        public RmqQueueBuilder<T> HostName(string name)
        {
            hostName = name;
            return this;
        }

        public RmqQueueBuilder<T> QueueName(string name)
        {
            queueName = name;
            return this;
        }

        public RmqQueueBuilder<T> Durable(bool durable)
        {
            this.durable = durable;
            return this;
        }

        public RmqQueueBuilder<T> Exclusive(bool exclusive)
        {
            this.exclusive = exclusive;
            return this;
        }

        public RmqQueueBuilder<T> AutoDelete(bool autoDelete)
        {
            this.autoDelete = autoDelete;
            return this;
        }

        public RmqQueueBuilder<T> Arguments(IDictionary<string, object> arguments)
        {
            this.arguments = arguments;
            return this;
        }
    }
}