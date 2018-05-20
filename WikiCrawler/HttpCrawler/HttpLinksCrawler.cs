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


namespace WikiCrawler.HttpCrawler
{
    public class HttpLinksCrawler : ICrawler, IDisposable
    {
        private readonly Subject<string> _urisSubject;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpLinksCrawler> _log;
        private readonly Func<string, bool> _isAcceptableUri;
        
        public IObservable<string> NewUris { get => _urisSubject; }

        public HttpLinksCrawler(Func<string, bool> isAcceptable)
        {
            _urisSubject = new Subject<string>();
            _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };
            _log = new LoggerFactory().CreateLogger<HttpLinksCrawler>();
            _isAcceptableUri = isAcceptable;
        }

        public HttpLinksCrawler(Func<string, bool> isAcceptable, ILogger<HttpLinksCrawler> logger)
            : this(isAcceptable)
        {
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
            if (!ValidParsedHttpUri(startUri)) {
                throw new ArgumentException("Invalid initial uri for crawling");
            }

            ISet<string> fetchedUris = new HashSet<string>() { startUri };

            while (fetchedUris.Count > 0)
            {
                ISet<string> loopedUris = new HashSet<string>();  // to avoid modifying of fetchedUris while iterating
                foreach (var uri in fetchedUris) {
                    try
                    {
                        string htmlPageCode = await _httpClient.GetStringAsync(uri);
                        loopedUris.Add(uri);

                        var htmlDoc = ParseHtmlCode(htmlPageCode);
                        var docUris = FetchHtmDocUris(baseUri: uri, htmlDoc);
                        foreach (var fetched in docUris) {
                            if (_isAcceptableUri(fetched)) {
                                _log.LogInformation($"Visited: {fetched}");
                                loopedUris.Add(fetched);
                                _urisSubject.OnNext(fetched);
                            }
                        }
                    } 
                    catch (AggregateException e)
                    { // Thrown when in _httpClient.GetStringAsync(uri) uri is invalid
                        _log.LogWarning($"Invalid uri: {e.Message}");
                    }
                    catch (HttpRequestException e)
                    { // Thrown then HttpClient connection timeout passed
                        _log.LogWarning($"Request not successed: {e.Message}");
                    }
                    catch (Exception e) {
                        _log.LogError($"Error while crawling: {e.Message}");
                        break;
                    }
                }

                fetchedUris.SymmetricExceptWith(loopedUris);  // Operation remove visited uris
            }

            _urisSubject.OnCompleted();
        }


        private HtmlDocument ParseHtmlCode(string htmlCode)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlCode);

            return htmlDoc;
        }

        
        private IEnumerable<string> FetchHtmDocUris(string baseUri, HtmlDocument htmlDoc)
        {
            var fetchedUris = new List<string>();
            IEnumerable<HtmlNode> refNodes = htmlDoc.DocumentNode.SelectNodes("//a");
            
            foreach (HtmlNode node in refNodes ?? Array.Empty<HtmlNode>())
            {
                var href = node.GetAttributeValue("href", null) ?? "";
                href = IsRelativeUri(href) ? baseUri + href : href;

                if (ValidParsedHttpUri(href)) {
                    fetchedUris.Add(href);
                }
            }

            return fetchedUris;
        }

        
        private bool ValidParsedHttpUri(string uri)
        {
            try
            {
                // If Uri instance created, uri is valid
                var parsedUri = new Uri(uri);

                if (uri != "#" && (parsedUri.Scheme == "http" || parsedUri.Scheme == "https"))
                {
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


        private bool IsRelativeUri(string uri)
        {
            return uri.StartsWith('/');
        }
    }




}