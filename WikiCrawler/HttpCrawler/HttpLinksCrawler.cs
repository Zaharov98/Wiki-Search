using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using WikiCrawler;
using WikiCrawler.SizedCache;


namespace WikiCrawler.HttpCrawler
{
    public class HttpLinksCrawler : ICrawler, IDisposable
    {
        private readonly Subject<string> _urisSubject;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpLinksCrawler> _log;
        
        public IObservable<string> NewUris { get => _urisSubject; }

        public HttpLinksCrawler(ILogger<HttpLinksCrawler> logger)
        {
            _urisSubject = new Subject<string>();
            _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };
            _log = logger;
        }

        
        public void Dispose()
        {
            _urisSubject.Dispose();
            _httpClient.Dispose();
        }

        public void Subscribe(Action<string> onNext, Action onComplete)
        {
            _urisSubject.Subscribe(onNext, onComplete);
        }

        
        public async Task StartCrawling(string startUri)
        {
            try
            {
                ISet<Uri> fetchedUris = new HashSet<Uri>() { new Uri(startUri) };
            
                while (fetchedUris.Count > 0)
                {
                    foreach (Uri uri in fetchedUris)
                    {
                        string htmlPageCode = await _httpClient.GetStringAsync(uri);
                        fetchedUris.Remove(uri);

                        var htmlDoc = ParseHtmlCode(htmlPageCode);
                        var docUris = FetchHtmDocUris(htmlDoc);
                        foreach (var fetched in docUris) {
                            _log.LogInformation($"Visited: {fetched}");

                            fetchedUris.Add(fetched);
                            _urisSubject.OnNext(fetched.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.LogError($"Exeption in HttpCrawler: {e.Message}");
                throw;
            }  
        }


        private HtmlDocument ParseHtmlCode(string htmlCode)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlCode);

            return htmlDoc;
        }

        
        private IEnumerable<Uri> FetchHtmDocUris(HtmlDocument htmlDoc)
        {
            var fetchedUris = new List<Uri>();
            IEnumerable<HtmlNode> refNodes = htmlDoc.DocumentNode.SelectNodes("//a");
            
            foreach (HtmlNode node in refNodes ?? Array.Empty<HtmlNode>())
            {
                var href = node.GetAttributeValue("href", null);
                if (ValidParsedUri(href)) {
                    fetchedUris.Add(new Uri(href));
                }
            }

            return fetchedUris;
        }

        
        private bool ValidParsedUri(string uri)
        {
            try
            {
                if (uri != null && uri != "#")
                {
                    // If Uri instance created, uri is valid
                    var parsedUri = new Uri(uri);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception) {
                return false;
            }   
        }
    }
}