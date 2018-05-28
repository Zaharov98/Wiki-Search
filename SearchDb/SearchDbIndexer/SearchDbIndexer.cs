using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Indexer
{
    public partial class SearchDbIndexer : IPageIndexer
    {
        private WordsDbContext _context;

        public SearchDbIndexer(WordsDbContext context)
        {
            _context = context;
        }


        public async Task<bool> AddToIndexAsync(string url, IEnumerable<string> links, IDictionary<string, IList<int>> wordLocations)
        {
            try
            {
                var urlObj = await AddOrFindUrlAsync(url);
                await AddWordsAsync(wordLocations.Keys);
                await AddWordsLocationsAsync(urlObj, wordLocations);
                await AddLinksAsync(urlObj, links);
                await AddUrlWordsAsync(links);

                return true;
            }
            catch (Exception e)
            {
                // not added to index
                Console.WriteLine(e.Message); // TODO: logging
                return false;
            }    
        }
    }
}
