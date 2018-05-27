using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using PageInfoCrawler.HtmlParse;

namespace PageInfoCrawler.SearchDbApiAccessor
{
    public class CrawlerApi
    {
        private static HttpClient client;
        private static IConfiguration configuration;

        public static void ConfigureStaticInstance(string configPath)
        {
            configuration = Program.BuildConfiguration(configPath);
            client = new HttpClient() { BaseAddress = new Uri(configuration["BaseAddress"]) };
        }

        public static async Task PostPageInfoAsync(byte[] body)
        {
            HttpContent content = new ByteArrayContent(body);
            await content.LoadIntoBufferAsync();

            // API connection configuration
            var requestUri = configuration["PostPageInfo"];
            await client.PostAsync(requestUri, content);
        }
    }
}
