using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SearchDbApi.Indexer;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Indexer
{
    public partial class SearchDbIndexer
    {
        private async Task AddWordsLocationsAsync(Url urlObj, IDictionary<string, IList<int>> wordLocations)
        {
            var locationsList = new List<WordLocation>();

            foreach (var word in wordLocations.Keys)
            {
                var wordObj = await AddOrFindWordAsync(word);
                foreach (var location in wordLocations[word])
                {
                    locationsList.Add(new WordLocation()
                    {
                        Word = wordObj,
                        Url = urlObj,
                        Location = location
                    });
                }
            }

            await _context.WordLocations.AddRangeAsync(locationsList);
            await _context.SaveChangesAsync();
        }


        private async Task<Word> AddOrFindWordAsync(string word)
        {
            var wordObj = await _context.Words.FindAsync(word);
            if (word == null)
            {
                var wordEntity = await _context.AddAsync(new Word() { Value = word });
                wordObj = wordEntity.Entity;
            }

            return wordObj;
        }
    }
}