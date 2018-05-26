using System;

namespace WeGouSharp
{
    public static class ServiceProviderAccessor
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void SetServiceProvider(IServiceProvider sp)
        {
            ServiceProvider = sp;
        }
    }
}
