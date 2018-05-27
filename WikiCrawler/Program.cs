﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NLog.Extensions;
using NLog.Extensions.Logging;
using WikiCrawler.HttpCrawler;
using WikiCrawler.WikiUriFilter;
using WikiCrawler.AmqpLinksSender;
using WikiCrawler.AmqpLinksSender.Builder;
using System.IO;

namespace WikiCrawler
{
    class Program
    {
        static IConfiguration Configuration { get; set; } = BuildConfiguration(@"../../../appsettings.json");


        static async Task Main(string[] args)
        {
            try
            {
                var logFactory = new NLogLoggerFactory();
                logFactory.ConfigureNLog(Configuration["NLog:Configuration"]);

                IConfiguration rabbitConf = BuildConfiguration(Configuration["RabbitMQ:Configuration"]);
                QueueSenderBuilder queueBuilder = new QueueSenderBuilder()
                    .HostName(rabbitConf["HostName"])
                    .QueueName(rabbitConf["QueueName"])
                    .Durable(bool.Parse(rabbitConf["Durable"]))
                    .Exclusive(bool.Parse(rabbitConf["Exclusive"]))
                    .AutoDelete(bool.Parse(rabbitConf["AutoDelete"]))
                    .Arguments(null)
                    .Logger(logFactory.CreateLogger<QueueSender>());

                EnglishWikiFilter filter = new EnglishWikiFilter();
                using (QueueSender sender = queueBuilder.Build())
                using (ICrawler crawler = new HttpLinksCrawler(filter.AcceptUri, logFactory.CreateLogger<HttpLinksCrawler>()))
                {
                    crawler.Subscribe(sender.SendToQueue, () => sender.Dispose());
                    await crawler.StartCrawling(Configuration["StartUri"]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exeption: {e.Message}; Trace: {e.StackTrace}");
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