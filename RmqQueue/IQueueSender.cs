using System;
using System.Collections.Generic;

namespace RmqQueue
{
    public interface IQueueSender<TMessage>
    {
        void SendToQueue(TMessage message);
    }
}