using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchDbApi.Search
{
    public interface ISearch
    {
        Task<IDictionary<string, double>> SearchUrlsAsync(string request);
    }
}
