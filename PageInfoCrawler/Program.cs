using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using MessagePack;
using PageInfoCrawler.AmqpLinksReciver;
using PageInfoCrawler.AmqpLinksReciver.Builder;
using PageInfoCrawler.HtmlParse;
using System.Net.Http;

namespace PageInfoCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var logFactory = new NLogLoggerFactory();
                logFactory.ConfigureNLog(@"../../../nlog.config");

                IConfiguration rabbitConf = BuildConfiguration(@"../../../rabbitmqConfig.json");
                QueueReciverBuilder queueBuilder = new QueueReciverBuilder()
                    .HostName(rabbitConf["HostName"])
                    .QueueName(rabbitConf["QueueName"])
                    .Durable(bool.Parse(rabbitConf["Durable"]))
                    .Exclusive(bool.Parse(rabbitConf["Exclusive"]))
                    .AutoDelete(bool.Parse(rabbitConf["AutoDelete"]))
                    .Arguments(null)
                    .Logger(logFactory.CreateLogger<QueueReciver>());


                using (var reciver = queueBuilder.Build())
                {
                    // TODO: Make OnNext handler
                    reciver.Subscribe(OnNext, () => Console.WriteLine("On Complete"));
                    reciver.StartReciving();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static HttpClient client = new HttpClient() { BaseAddress = new Uri("http://localhost:50494") };

        private static async void OnNext(string url)
        {
            try
            {
                var pageItems = await HtmlPageParser.GetPageItems(url);
                var pageItemsSerialized = MessagePackSerializer.Serialize(pageItems);

                // TODO: application/msgpack
                HttpContent content = new ByteArrayContent(pageItemsSerialized);
                await content.LoadIntoBufferAsync();

                // API connection configuration
                var requestUri = "http://localhost:50494/api/v1/crawler";
                await client.PostAsync(requestUri, content);

                Console.WriteLine($"{pageItems.PageUrl};  Links count: {pageItems.Links.Count}");
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        private static IConfiguration BuildConfiguration(string confFilePath)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Environment.CurrentDirectory);
            configBuilder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, confFilePath));

            return configBuilder.Build();
        }
    }
}
