using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace WeGouSharpPlus
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! dotnet core");
            //加载log4net配置
            LoadLoggerConfig();
            LogHelper.logger.Debug("Hello WeGouSharp");
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
