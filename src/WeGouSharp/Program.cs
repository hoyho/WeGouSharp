using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using WeGouSharp.Model;
using WeGouSharp.YunDaMa;
using WeGouSharp.Infrastructure;
using System.Threading;

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

        public static void RegisterOnExit()
        {
            //注册中断按钮，否则下面的ProcessExit 事件不一定能触发
            RegisterExitKey();

            //somet time this not works ,for example ^C or taskkill, thus also register  at ControlBreak event
            System.AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Console.WriteLine("goodbye wegousharp");

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //run powershell to kill
                    var gracefulShutdown = @"$
                    $geckodrivers=(ps -Name geckodriver)
                    Stop-Process -InputObject $geckodrivers

                    #list all firefox instance
                    $all_ff=(ps -Name firefox | select id,ProcessName,mainWindowTItle)
                    echo $all_ff

                    #headless firefox mainWindowTItle=''
                    $headless_firefox=(ps -Name firefox| Where { $_.mainWindowTItle -eq '' } )
                    Stop-Process -InputObject $headless_firefox";
                    gracefulShutdown.RunAsShell(isPowerShell:true);
                    return;
                }

                //kill geckodriver , and geckodriver.sh will kill himself and kill firefox instance`
                //var  killRunningGeckoCmd = "ps -ef | grep \"firefox\" | grep  \"marionette\"  | awk {'print \"kill \" $2'} | bash ";
                var killGeckoShellCmd = "ps -ef | grep geckodriver | grep port | awk {'print \"kill \" $2'} | bash ";

                //killRunningGeckoCmd.RunAsShell();
                killGeckoShellCmd.RunAsShell();

            };



        }

        private static void RegisterExitKey()
        {
           //ref http://geekswithblogs.net/akraus1/archive/2006/10/30/95435.aspx
            Console.CancelKeyPress +=  (s, e) =>
            {

                if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
                {
                    Console.WriteLine("Ctrl-Break catched");
                    // Envirionment.Exit(1) would NOT do a cooperative shutdown. No finalizers are called!
                }
                if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                {
                     Console.WriteLine("Ctrl-C catched ");
                }

                //On windows 10, program catch calcle signal, but not handle , so clean up and exit program manully
                 if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //run powershell to kill
                    var gracefulShutdown = @"$
                    $geckodrivers=(ps -Name geckodriver)
                    Stop-Process -InputObject $geckodrivers

                    #list all firefox instance
                    $all_ff=(ps -Name firefox | select id,ProcessName,mainWindowTItle)
                    echo $all_ff

                    #headless firefox mainWindowTItle=''
                    $headless_firefox=(ps -Name firefox| Where { $_.mainWindowTItle -eq '' } )
                    Stop-Process -InputObject $headless_firefox";
                    gracefulShutdown.RunAsShell(isPowerShell:true);
                   // return;
                    Environment.Exit(0);
                    
                }


            };

            Console.WriteLine("Press Ctrl-C or Ctrl-Break to exit ");
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