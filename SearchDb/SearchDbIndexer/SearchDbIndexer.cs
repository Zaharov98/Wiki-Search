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
                using (_context)
                {// No sence to try to aviod some data dublication in this context
                 // so the ALTER TABLE [TableName] REBUILD WITH (IGNORE_DUP_KEY = ON);
                 // was used
                    var urlObj = await FindUrlAsync(url);
                    if (urlObj == null || urlObj.Indexed == false) 
                    {
                        if (urlObj == null) {
                            urlObj = await AddUrlAsync(url);
                        } else {
                            urlObj.Indexed = true;
                        }    

                        await AddWordsAsync(wordLocations.Keys);
                        await AddWordsLocationsAsync(urlObj, wordLocations);
                        await AddLinksAsync(urlObj, links);
                        await AddUrlWordsAsync(url);

                        await _context.SaveChangesAsync();
                    }
                }
                Console.WriteLine(url); //Ultra logging
                return true;
            }
            catch (Exception e)
            {
                // not added to index
                Console.WriteLine(e.Message); // TODO: logging
                return false;
            }    
        }


        private async Task<Url> FindUrlAsync(string url)
        {
            return await _context.Urls.FindAsync(url);
        }

        private async Task<Url> AddUrlAsync(string url)
        {
            var urlEntity = await _context.Urls.AddAsync(new Url()
            {
                Value = url,
                Indexed = true
            });

            return urlEntity.Entity;
        }
    }
}
