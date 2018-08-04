using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeGouSharp;
using WeGouSharp.Model;
using WeGouSharp.YunDaMa;

namespace WeGouTest
{
    /// <summary>
    /// 基本配置，注入等初始化
    /// </summary>
    public class BasicConfig
    {
        protected readonly WeGouService ApiService;

        protected BasicConfig()
        {
//
//            //init logger
//            FileInfo configFile = new FileInfo("log4net.config");
//            var repo = LogManager.GetRepository(Assembly.GetEntryAssembly());
//            XmlConfigurator.ConfigureAndWatch(repo, configFile);
//
//
//            // //创建logger
//            var logger = LogManager.GetLogger(typeof(Program));
//
//            // Set up configuration sources.
//            var cfBuilder = new ConfigurationBuilder()
//                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
//                .AddJsonFile("wegousharpsettings.json",false)
//                .AddJsonFile("appsettings.json", false);
//
//            var config = cfBuilder.Build();
//
//            var sp = new ServiceCollection()
//            .AddSingleton<IConfiguration>(config)
//            .AddSingleton(logger)
//            .AddSingleton(config.GetSection("YunDaMa").Get<YunDaMaConfig>())
//            .AddScoped<IDecode, OnlineDecoder>()
//            .AddScoped<WeGouService,WeGouService>()
//            .AddSingleton<Browser, Browser>()
//            .BuildServiceProvider();
//
//
//            ServiceProviderAccessor.SetServiceProvider(sp);
//
//            var bs = ServiceProviderAccessor.ResolveService<Browser>();
//
//
//            // var ydmConfig = configuration.GetSection("YunDaMa").Get<YunDaMaConfig>();
//
//            // var yunDaMa = new OnlineDecoder(ydmConfig);
//
//             ApiService = ServiceProviderAccessor.ResolveService<WeGouService>();
            ApiService = new WeGouService();
        }
    }
}