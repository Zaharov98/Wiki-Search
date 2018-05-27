﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using PageInfoCrawler.AmqpLinksReciver;
using PageInfoCrawler.AmqpLinksReciver.Builder;

namespace PageInfoCrawler
{
    class Program
    {
        public static IConfiguration Configuration { get; set; } = BuildConfiguration(@"../../../appsettings.json");

        public static NLogLoggerFactory LoggerFactory = new NLogLoggerFactory();


        static void Main(string[] args)
        {
            try
            {
                LoggerFactory.ConfigureNLog(Configuration["NLog:Configuration"]);

                IConfiguration rabbitConf = BuildConfiguration(Configuration["RabbitMQ:Configuration"]);
                QueueReciverBuilder queueBuilder = new QueueReciverBuilder()
                    .HostName(rabbitConf["HostName"])
                    .QueueName(rabbitConf["QueueName"])
                    .Durable(bool.Parse(rabbitConf["Durable"]))
                    .Exclusive(bool.Parse(rabbitConf["Exclusive"]))
                    .AutoDelete(bool.Parse(rabbitConf["AutoDelete"]))
                    .Arguments(null)
                    .Logger(LoggerFactory.CreateLogger<QueueReciver>());

                using (var reciver = queueBuilder.Build())
                {
                    reciver.Subscribe(QueueMessageHandler.OnNext, () => Console.WriteLine("On Complete"));
                    reciver.StartReciving();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static IConfiguration BuildConfiguration(string confFilePath)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Environment.CurrentDirectory);
            configBuilder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, confFilePath));

            return configBuilder.Build();
        }
    }
}
