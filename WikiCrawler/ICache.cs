using System;
using System.Collections.Generic;
using System.Text;

namespace WikiCrawler
{
    public interface ICache<T>
    {
        void Add(T item);
        bool Contains(T item);
    }
}
