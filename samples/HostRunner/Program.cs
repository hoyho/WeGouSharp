using System;
using WeGouSharp;

namespace HostRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

             var apiService =  WeGouService.Instance;

            var bs = ServiceProviderAccessor.ResolveService<Browser>();

            var rs = apiService.GetAccountInfoByIdAsync("bitsea").Result;


            var rss = apiService.GetAccountInfoByIdSerializedAsync("bitsea").Result;
            Console.WriteLine(rss);
            //var ws = new WeGouService(logger,configuration,yunDaMa);

            //var rs = ws.GetOfficialAccountMessagesByName("gzhu");

            //var rs = ws.GetOfficialAccountMessagesByName("广州大学");

            Console.ReadKey();

         }
    }
}
