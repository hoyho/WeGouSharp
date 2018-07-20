using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using static WeGouSharp.Tools;
using System.Linq;
using System.Runtime.InteropServices;
using WeGouSharp.YunDaMa;
using System.Threading.Tasks;

namespace WeGouSharp
{


    public class Brower
    {

        ILog _logger;
        public Brower(ILog logger)
        {
            _logger = logger;
        }


        //输入网址返回页面内容
        public string Get(string url)
        {
            return "";
        }


        //微信文章页出现的验证码
        public Task HandleWxVcode(string url)
        {
            return Task.CompletedTask;
        }


        //搜狗页的验证码
        public Task HandleSogouVcode(string url)
        {
            return Task.CompletedTask;
        }

    }
}