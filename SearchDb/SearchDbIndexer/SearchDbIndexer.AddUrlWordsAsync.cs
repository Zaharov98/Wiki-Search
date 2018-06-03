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
        private async Task AddUrlWordsAsync(string url)
        {
            ISet<string> addedWords = new HashSet<string>();  // To aviod duplication

            var urlObj = await _context.Urls.FindAsync(url);
            foreach (var word in FetchWords(url)) {
                var wordObj = await _context.Words.FindAsync(word);

                if (wordObj == null && !addedWords.Contains(word))
                {// Insert into database
                    await _context.Words.AddAsync(new Word() { Value = word });
                    wordObj = await _context.Words.FindAsync(word);

                    addedWords.Add(word); // To avoid PK duplication
                }

                await _context.UrlWords.AddAsync(new UrlWord()
                {
                    Url = urlObj,
                    Word = wordObj,
                });
            }
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