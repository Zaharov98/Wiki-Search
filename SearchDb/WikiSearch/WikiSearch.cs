using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;
using SearchDbApi.Data.Sql;

namespace SearchDbApi.Search
{
    public class WikiSearch : ISearch
    {
        private readonly SqlReader _reader;
        private readonly WordsDbContext _context;

        public WikiSearch(SqlReader reader, WordsDbContext context)
        {
            this._reader = reader;
            this._context = context;
        }

        public async Task<IDictionary<string, double>> SearchUrlsAsync(string request)
        {
            // TODO: make complete search
            // TODO: refactor
            var words = new HashSet<string>(request.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            var rows = await GetMatchRowsAsync(words);

            if (rows.Count > 0) {
                var scoredDict = await GetScoredListAsync(rows, words.ToList());
                return scoredDict;
            } else {
                return new Dictionary<string, double>();
            }
        }

        private async Task<IDictionary<string, double>> GetScoredListAsync(IList<(string, IList<int>)> rows, IList<string> words)
        {
            var totalScores = new Dictionary<string, double>();
            foreach (var row in rows) {
                totalScores.TryAdd(row.Item1, 0);
            }

            var weights = new List<(double, IDictionary<string, double>)>()
            {
                (1.0, FrequencyScore(rows)),
                (1.0, LocationScore(rows)),
                (1.0, DistanceScore(rows)),
                (1.0, await InboundLinkScore(rows)),
                (1.0, await LinkTextScore(rows, words))
            };

            foreach (var item in weights) {
                foreach (var url in totalScores.Keys.ToList()) {
                    totalScores[url] += item.Item1 * item.Item2[url];
                }
            }

            return totalScores;
        }

        private async Task<IList<(string, IList<int>)>> GetMatchRowsAsync(IEnumerable<string> words)
        {
            var fullQuery = await MatchRowQuery(words);
            if (fullQuery != null) {
                using (_reader)
                {
                    var queryResult = await _reader.ExecuteAsync(fullQuery);
                    var resultList = ResultList(queryResult);

                    return resultList;
                }
            } else {
                return new List<(string, IList<int>)>();
            }
        }

        private async Task<string> MatchRowQuery(IEnumerable<string> words)
        {
            // TODO: Refactor!!!
            var fieldList = "w0.UrlId";
            var tableList = "";
            var clauseList = "";
            var tableNumber = 0;

            int indexedWordsCount = 0;
            foreach (var word in words)
            {
                var wordObj = await _context.Words.FindAsync(word);
                if (wordObj != null)
                {
                    indexedWordsCount++;

                    if (tableNumber > 0)
                    {
                        tableList += ", ";
                        clauseList += " AND ";
                        clauseList += String.Format("w{0}.UrlId = w{1}.UrlId AND ", tableNumber - 1, tableNumber);
                    }

                    fieldList += $", w{tableNumber}.Location";
                    tableList += $"WordLocations w{tableNumber}";
                    clauseList += $"w{tableNumber}.WordId = '{word}'";
                    tableNumber += 1;
                }
            }

            if (indexedWordsCount > 0) {
                return $"SELECT {fieldList} FROM {tableList} WHERE {clauseList}";
            } else {
                return null;
            }
        }

        private List<(string, IList<int>)> ResultList(IList<IList<object>> rows)
        {
            var resultList = new List<(string, IList<int>)>();
            foreach (var row in rows) {
                string uri = row[0] as string;

                var locations = new List<int>();
                for (int i = 1; i < row.Count; ++i) {
                    locations.Add((int)row[i]);
                }

                resultList.Add((uri, locations));
            }

            return resultList;
        }

        private IDictionary<string, double> FrequencyScore(IList<(string, IList<int>)> rows)
        {
            var countsDict = new Dictionary<string, double>();
            foreach (var row in rows) {
                countsDict.TryAdd(row.Item1, 0);
            }

            foreach (var row in rows) {
                countsDict[row.Item1] += 1;
            }

            return NormalizeScores(countsDict);
        }

        private IDictionary<string, double> LocationScore(IList<(string, IList<int>)> rows)
        {
            var locationsDict = new Dictionary<string, double>();
            foreach (var row in rows) {
                locationsDict.TryAdd(row.Item1, double.MaxValue);
            }

            foreach (var row in rows) {
                var local = row.Item2.Sum();
                if (local < locationsDict[row.Item1]) {
                    locationsDict[row.Item1] = local;
                }
            }

            return NormalizeScores(locationsDict, smallIsBetter: true);
        }

        private IDictionary<string, double> DistanceScore(IList<(string, IList<int>)> rows)
        {
            var minDistanceDict = new Dictionary<string, double>();

            if (rows[0].Item2.Count < 2) { // One word in request
                foreach (var row in rows) {
                    minDistanceDict.TryAdd(row.Item1, 0);
                }
                return minDistanceDict;
            }

            foreach (var row in rows) {
                minDistanceDict.TryAdd(row.Item1, double.MaxValue);
            }

            foreach (var row in rows) {
                var distances = new List<double>();
                for (int i = 1; i < row.Item2.Count; ++i) {
                    distances.Add(Math.Abs(row.Item2[i] - row.Item2[i - 1]));
                }

                var minDistance = distances.Min();
                if (minDistance < minDistanceDict[row.Item1]) {
                    minDistanceDict[row.Item1] = minDistance;
                }
            }

            return NormalizeScores(minDistanceDict, smallIsBetter: true);
        }

        private async Task<IDictionary<string, double>> InboundLinkScore(IList<(string, IList<int>)> rows)
        {
            var urlSet = new HashSet<string>();
            foreach (var row in rows) {
                urlSet.Add(row.Item1);
            }

            var countDict = new Dictionary<string, double>();
            foreach (var url in urlSet) {
                var count = await _context.Links.CountAsync(link => link.ToUrlId == url);
                countDict.TryAdd(url, count);
            }

            return NormalizeScores(countDict);
        }

        private async Task<IDictionary<string, double>> LinkTextScore(IList<(string, IList<int>)> rows, IList<string> words)
        {
            var urlScoreDict = new Dictionary<string, double>();
            foreach (var row in rows) {
                urlScoreDict.TryAdd(row.Item1, 0);
            }

            var scoreIncrement = 1 / words.Count;
            foreach (var word in words) {
                foreach (var url in urlScoreDict.Keys.ToList()) {
                    if (await _context.UrlWords
                        .CountAsync(u => u.UrlId == url && u.WordId == word) > 0)
                    {
                        urlScoreDict[url] += scoreIncrement; 
                    }
                }
            }

            return NormalizeScores(urlScoreDict);
        }

        private IDictionary<string, double> NormalizeScores(IDictionary<string, double> scores, bool smallIsBetter = false)
        {
            var normalizedDict = new Dictionary<string, double>();

            var verySmall = 0.00001;
            if (smallIsBetter) {
                var minScore = scores.Values.Min();
                foreach (var pair in scores) {
                    normalizedDict.TryAdd(pair.Key, pair.Value / Math.Max(verySmall, pair.Value));
                }
            } else {
                var maxScore = scores.Values.Max();
                foreach (var pair in scores) {
                    normalizedDict.TryAdd(pair.Key, maxScore != 0 ? pair.Value / maxScore : 0);
                }
            }

            return normalizedDict;
        }
    }
}
