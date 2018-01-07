using log4net;
using log4net.Config;
using System;
using System.IO;

namespace WeGouSharpPlus
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! dotnet core");
            //加载log4net配置
            FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            //tofix
            //XmlConfigurator.ConfigureAndWatch(new log4net.Repository.ILoggerRepository(),configFile);
            //创建logger
            var logger = LogManager.GetLogger(typeof(Program));

            //调用示例
            Test.run();
        }
    }
}
