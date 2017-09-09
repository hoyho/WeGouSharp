using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeGouSharp
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //加载log4net配置
            FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(configFile);
            //创建logger
             var logger = LogManager.GetLogger(typeof(Program));
            logger.Warn("warnlog1");
            var WechatCache = new WechatCache(Config.CacheDir, 3);
            var f1= WechatCache.Add("cache1kEY键", "wechatCacheValue", 300);
            string wechatCacheValue = WechatCache.Get<object>("cache1kEY键").ToString();
            string wechatCacheValue2 = Convert.ToString(WechatCache.Get<object>("cache1kEY"));

            var has1 = WechatCache.Has("cache1kEY");
            var has2 = WechatCache.Has("cache1kEY键");

            var WechatCache2 = WechatCache.Update("cache1kEY键", "wechatCacheValue-update",1);
             wechatCacheValue =WechatCache.Get<object>("cache1kEY键").ToString();

            var WechatCache3 = WechatCache.Update("not exit", "", 1);
            var wec = WechatCache.ClearAll();

            WechatSogouApi Sogou = new WechatSogouApi();
            var result = Sogou.GetOfficialAccountMessages("","bitsea",""); // get_gzh_message
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Console.Write(json);
            Console.ReadKey();
        }
    }
}
