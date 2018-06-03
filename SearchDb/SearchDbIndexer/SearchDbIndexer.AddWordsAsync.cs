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
            ISet<string> addedWords = new HashSet<string>();

            foreach (var word in words) {
                if (IsFromLetters(word)) {
                    var searchRes = await _context.Words.FindAsync(word);
                    if (searchRes == null  && !addedWords.Contains(word)) {
                        wordsList.Add(new Word() { Value = word });
                        addedWords.Add(word); // To avoid PK duplication
                    }
                }
            }
            
            await _context.Words.AddRangeAsync(wordsList);
        }

        private bool IsFromLetters(string word)
        {
            foreach (var symb in word) {
                if (!char.IsLetter(symb)) {
                    return false;
                }
            }
            return true;
        }
    }
}