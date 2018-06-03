using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using  WeGouSharp;
using WeGouSharp.Model;
using WeGouSharp.YunDaMa;

namespace HostRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            
            //init logger
            var configFile = new FileInfo("log4net.config");
            var repo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(repo, configFile);
            
            
            // //创建logger
            var logger = LogManager.GetLogger(typeof(Program));
            
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json",false);
            
            IConfiguration configuration = builder.Build();


            var ydmConfig = configuration.GetSection("YunDaMa").Get<YunDaMaConfig>();

            var yunDaMa = new OnlineDecoder(ydmConfig);

            
            var ws = new WeGouService(logger,configuration,yunDaMa);

            //var rs = ws.GetOfficialAccountMessagesByName("gzhu");

            var rs = ws.GetOfficialAccountMessagesByName("广州大学");

            Console.ReadKey();

        }
    }
}
