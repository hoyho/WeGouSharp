using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
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
            
            //init logger
            FileInfo configFile = new FileInfo("log4net.config");
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
            
            ApiService = new WeGouService(logger,configuration,yunDaMa);
        }
    }
}