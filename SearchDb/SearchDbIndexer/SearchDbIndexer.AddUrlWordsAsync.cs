using System;
using System.Text;
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
        private async Task AddUrlWordsAsync(IEnumerable<string> links)
        {
            foreach (var link in links) {
                var linkObj = await _context.Urls.FindAsync(link);
                foreach (var word in FetchWords(link)) {
                    var wordObj = await _context.Words.FindAsync(word);
                    if (wordObj == null) {
                        await _context.Words.AddAsync(new Word() { Value = word });
                        await _context.SaveChangesAsync();
                        wordObj = await _context.Words.FindAsync(word);
                    }

                    await _context.UrlWords.AddAsync(new UrlWord()
                    {
                        Url = linkObj,
                        Word = wordObj,
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private IList<string> FetchWords(string url)
        {
            var words = new List<string>();

            int i = 0;
            while (i < url.Length)
            {
                if (char.IsLetter(url[i])) {
                    var accumulator = new StringBuilder();
                    while(i < url.Length && char.IsLetter(url[i])) {
                        accumulator.Append(url[i]);
                        ++i;
                    }
                    words.Add(accumulator.ToString().ToLowerInvariant());
                }

                ++i;
            }

            return words;
        }
    }
}