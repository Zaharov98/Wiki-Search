using System;
using System.Collections.Generic;

namespace RmqQueue
{
    public interface IQueueReciver<TMessage>
    {
        void StartReciving();
    }
}