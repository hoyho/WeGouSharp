using System;

namespace WeGouSharp
{
    /// <summary>
    /// 注入服务和管理
    /// </summary>
    public static class ServiceProviderAccessor
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        
        public static void SetServiceProvider(IServiceProvider sp)
        {
            ServiceProvider = sp;
        }

        public static T ResolveService<T>() where T:class
        {
         return (T) ServiceProvider.GetService(typeof(T));
        }
    }
}
