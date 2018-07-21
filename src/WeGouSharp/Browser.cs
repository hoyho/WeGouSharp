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
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;
using System.Collections.Generic;
using OpenQA.Selenium;
using System.Threading;
using WeGouSharp.Model.OS;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;

namespace WeGouSharp
{
    public class Browser
    {

        private FirefoxDriver _driver;

        private List<string> _tabs = new List<string>();

        IConfiguration _config;

        ILog _logger;
        public Browser(ILog logger)
        {
            _logger = logger;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FirefoxProfile fxProfile = new FirefoxProfile();
            fxProfile.SetPreference("browser.download.folderList", 1); //0:desktop 1:download folder 2:custom
            fxProfile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "text/plain");

            FirefoxOptions fxops = new FirefoxOptions() { Profile = fxProfile };
            _driver = (FirefoxDriver)LaunchFireFox(fxops);

            var homeAddr = "http://weixin.sogou.com/antispider/?from=%2fweixin%3Ftype%3d2%26query%3d%E5%B9%BF%E5%B7%9E%E5%A4%A7%E5%AD%A6%26ie%3dutf8%26s_from%3dinput%26_sug_%3dn%26_sug_type_%3d1%26w%3d01015002%26oq%3d%26ri%3d6%26sourceid%3dsugg%26sut%3d0%26sst0%3d1532192838174%26lkt%3d0%2C0%2C0%26p%3d40040108";
            _driver.Navigate().GoToUrl(homeAddr);

            var ele = _driver.FindElementById("seccodeImage");
            var base64string = _driver.ExecuteScript(@"
    var c = document.createElement('canvas');
    var ctx = c.getContext('2d');
    var img = document.getElementById('seccodeImage');
    c.height=img.height;
    c.width=img.width;
    ctx.drawImage(img, 0, 0,img.width, img.height);
    var base64String = c.toDataURL();
    return base64String;
    ") as string;

            var base64 = base64string.Split(',').Last();
            Console.WriteLine(_driver.PageSource);
            _tabs = _driver.WindowHandles.ToList();

        }



        public FirefoxDriver LaunchFireFox(FirefoxOptions option)
        {
            var browserPath = "";
            //folder containe geckodriver            
            var geckodriverPath = "/home/hoyho/workspace/WeGouSharp/src/WeGouSharp/Resource/firefox_linux/";

            FirefoxDriver driver = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) //linux and have desktop
            {
                //path = _config["FireFoxPath_Linux"];
                browserPath = "/home/hoyho/workspace/WeGouSharp/src/WeGouSharp/Resource/firefox_linux/firefox";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                browserPath = _config["FireFoxPath_OSX"];
            }
            else
            {
                browserPath = _config["FireFoxPath_Windows"];
            }
            var fds = FirefoxDriverService.CreateDefaultService(geckodriverPath);
            //fds.FirefoxBinaryPath = browserPath;
            option.AddArgument("-headless");

            driver = new FirefoxDriver(fds, option, TimeSpan.FromMinutes(1));

            return driver;
        }


        //输入网址返回页面内容
        public Task<string> GetAsync(string url)
        {
            return Task.Run(() =>
             {
                 _driver.Navigate().GoToUrl(url);
                 return _driver.PageSource;
             });

        }


        //微信文章页出现的验证码
        public async Task HandleWxVcodeAsync(string vCodeUrl)
        {

            var vcodePage = await GetAsync(vCodeUrl);
        }


        //搜狗页的验证码
        public Task HandleSogouVcode(string vCodeUrl)
        {
            return Task.CompletedTask;
        }

    }
}