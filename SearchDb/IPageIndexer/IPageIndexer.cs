using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchDbApi.Indexer
{
    public interface IPageIndexer
    {
        Task<bool> AddToIndexAsync(string uri, IEnumerable<string> links, IDictionary<string, IList<int>> wordLocations);
    }
}
