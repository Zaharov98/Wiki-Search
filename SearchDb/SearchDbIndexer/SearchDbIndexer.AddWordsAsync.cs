using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SearchDbApi.Indexer;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Indexer
{
    public partial class SearchDbIndexer : IPageIndexer
    {
        private async Task AddWordsAsync(IEnumerable<string> words)
        {
            var wordsList = new List<Word>();
            foreach (var word in words) {
                var searchRes = await _context.Words.FindAsync(word);
                if (searchRes == null) {
                    wordsList.Add(new Word() { Value = word });
                }
            }

            await _context.Words.AddRangeAsync(wordsList);
            await _context.SaveChangesAsync();
        }
    }
}