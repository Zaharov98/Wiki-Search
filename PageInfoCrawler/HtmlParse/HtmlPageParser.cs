using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using MessagePack;

namespace PageInfoCrawler.HtmlParse
{
    public class HtmlPageParser
    {
        private static string[] _ignoreWords = new string[]{ "the", "and", "to", "a", "in", "is", "it" };
        private static HttpClient _httpClient = new HttpClient();

        public static async Task<HtmlPageItems> GetPageItems(string url)
        {
            try
            {
                string htmlPageCode = await _httpClient.GetStringAsync(url);
                var htmlDoc = ParseHtmlCode(htmlPageCode);

                IEnumerable<string> links = FetchHtmDocUris(baseUri: url, htmlDoc: htmlDoc);
                string pageText = FectHtmlDocText(htmlDoc);

                var wordLocations = WordsLocationDict(pageText);

                var pageItems = new HtmlPageItems()
                {
                    PageUrl = url,
                    Links = links.ToList(),
                    WordLocations = wordLocations
                };

                return pageItems;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static IDictionary<string, List<int>> WordsLocationDict(string text)
        {
            var wordLocationDict = new Dictionary<string, List<int>>();
            var wordAcumulator = new StringBuilder();

            int idx = 0;
            while (idx < text.Length)
            {
                if (char.IsLetterOrDigit(text[idx])) {
                    var startPosition = idx;    

                    wordAcumulator.Length = 0; // Flush old data
                    while (idx < text.Length && char.IsLetterOrDigit(text[idx])) {
                        wordAcumulator.Append(text[idx]);
                        ++idx;
                    }

                    string word = wordAcumulator.ToString();
                    if (!_ignoreWords.Contains(word)) {
                        if (wordLocationDict.ContainsKey(word)) {
                            wordLocationDict[word].Add(startPosition);
                        } else {
                            wordLocationDict.Add(word, new List<int>() { startPosition });
                        }
                    }
                }

                ++idx;
            }

            return wordLocationDict;
        }


        public static HtmlDocument ParseHtmlCode(string htmlCode)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlCode);

            return htmlDoc;
        }


        public static IEnumerable<string> FetchHtmDocUris(string baseUri, HtmlDocument htmlDoc)
        {
            var fetchedUris = new List<string>();
            IEnumerable<HtmlNode> refNodes = htmlDoc.DocumentNode.SelectNodes("//a");

            foreach (HtmlNode node in refNodes ?? Array.Empty<HtmlNode>())
            {
                var href = node.GetAttributeValue("href", null) ?? "";
                href = IsRelativeUri(href) ? baseUri + href : href;

                if (ValidParsedHttpUri(href))
                {
                    fetchedUris.Add(href);
                }
            }

            return fetchedUris;
        }

        
        public static string FectHtmlDocText(HtmlDocument htmlDoc)
        {
            var pageTextBuilder = new StringBuilder();

            foreach (var node in htmlDoc.DocumentNode.ChildNodes)
            {
                if (node.InnerText.Length > 0) {
                    var split = node.InnerText.Split(new char[] { ' ', ',', '.', '!', '?'}, 
                        StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in split) {
                        var word = item.Trim().ToLowerInvariant();
                        if (word.Length > 0) {
                            pageTextBuilder.Append(word);
                            pageTextBuilder.Append(' ');
                        }
                    }
                }
            }

            return pageTextBuilder.ToString();
        }


        public static bool IsRelativeUri(string uri) => uri.StartsWith('/');
    

        public static bool ValidParsedHttpUri(string uri)
        {
            try {
                // If Uri instance created, uri is valid
                var parsedUri = new Uri(uri);

                if (uri != "#" && (parsedUri.Scheme == "http" || parsedUri.Scheme == "https")) {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch (Exception) {
                return false;
            }
        }
    }    
}