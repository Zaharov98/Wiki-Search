using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using MessagePack;
using PageInfoCrawler.SearchDbApiAccessor;
using PageInfoCrawler.HtmlParse;

namespace PageInfoCrawler
{
    internal class QueueMessageHandler
    {
        private static ILogger<QueueMessageHandler> logger;

        static QueueMessageHandler()
        {
            // Global Db api connection antrypoint, using in OnNext handler
            CrawlerApi.ConfigureStaticInstance(Program.Configuration["CrawlerApi:Configuration"]); 
            
            logger = Program.LoggerFactory.CreateLogger<QueueMessageHandler>();
        }
        
        public static async void OnNext(string url)
        {
            try
            {
                Console.WriteLine(url); // Ultra logging
                var pageItems = await HtmlPageParser.GetPageItems(url);
                var pageItemsSerialized = MessagePackSerializer.Serialize(pageItems);
                await CrawlerApi.PostPageInfoAsync(pageItemsSerialized);

                logger.LogInformation($"Page getted: {url}");
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
}
