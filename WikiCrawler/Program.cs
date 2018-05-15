using System;
using Microsoft.Extensions.Logging;
using WikiCrawler.HttpCrawler;

namespace WikiCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<HttpLinksCrawler> logger = loggerFactory.CreateLogger<HttpLinksCrawler>();
            
            using (ICrawler crawler = new HttpLinksCrawler(logger))
            {
                crawler.Subscribe(x => Console.WriteLine($"Visited: {x}"), () => Console.WriteLine("On Complete"));
                crawler.StartCrawling("https://en.wikipedia.org/wiki/Computer_programming").Wait();
            }
            
            Console.WriteLine("Hello World!");
        }
    }
}