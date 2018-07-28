using Microsoft.Extensions.Configuration;

namespace WeGouSharp
{
    class Config
    {
        //缓存配置
        public static string CacheDir = "cache";

        public static string CacheSessionName = "requests_wechatsogou_session";

        public static IConfiguration Configuration =>
            ServiceProviderAccessor.ServiceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
        
    }
}
