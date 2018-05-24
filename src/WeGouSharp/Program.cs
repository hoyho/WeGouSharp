using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WeGouSharp
{
   public class Program
    {
        public static IConfigurationRoot _Configuration;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! dotnet core");
            //加载log4net配置
            LoadLoggerConfig();
            LogHelper.logger.Debug("Hello WeGouSharp");
            
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true);
            var configuration = builder.Build();
            _Configuration = configuration;
            var sp = new ServiceCollection().AddSingleton<IConfiguration>(configuration)
                .BuildServiceProvider();
//            var containerBuilder = new ContainerBuilder();
//            containerBuilder.register
            var myConf = sp.GetService<IConfiguration>();
            //调用示例
            Test.run();
        }
 
        //初始化log4net配置
        public static void LoadLoggerConfig()
        {
            //init logger
            FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            var repo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(repo, configFile);
            // //创建logger
            var logger = LogManager.GetLogger(typeof(Program));
            logger.Error("logger inited");
            
        }


    }
}
