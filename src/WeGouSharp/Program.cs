using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeGouSharp.Model;
using WeGouSharp.YunDaMa;

namespace WeGouSharp
{
    public class Program
    {
        private static bool _isInit;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! dotnet core");
            var srv = new WeGouService();
            var rs = srv.GetAccountInfoByIdAsync("bitsea").Result;
            Console.WriteLine(rs.AccountPageurl);
        }

        //初始化log4net配置
        private static void LoadLoggerConfig()
        {
            //init logger
            FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            var repo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(repo, configFile);
            // //创建logger
            var logger = LogManager.GetLogger(typeof(Program));
            logger.Error("logger inited");
        }


        /// <summary>
        /// Inject required service once in lifetime
        /// </summary>
        public static void EnsureInject()
        {
            if (_isInit) return;

            //init logger
            LoadLoggerConfig();

            // //创建logger
            var logger = LogManager.GetLogger(typeof(Program));
            logger.Debug("Program start");

            // Set up configuration sources.
            var cfBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                //.AddJsonFile("appsettings.json", false)
                .AddJsonFile("wegousharpsettings.json", false);
            var config = cfBuilder.Build();

            var sp = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddSingleton(logger)
                .AddSingleton(config.GetSection("YunDaMa").Get<YunDaMaConfig>())
                .AddScoped<IDecoder, OnlineDecoder>()
                .AddScoped<WeGouService, WeGouService>()
                .AddSingleton<Browser, Browser>()
                .BuildServiceProvider();

            ServiceProviderAccessor.SetServiceProvider(sp);

            _isInit = true;
        }
    }
}