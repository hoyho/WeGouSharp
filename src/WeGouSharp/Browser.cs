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
using System.Collections.Generic;
using OpenQA.Selenium;
using System.Threading;
using WeGouSharp.Model.OS;
using Microsoft.Extensions.Configuration;

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
            _driver = (FirefoxDriver)CreateFireFoxBrowser(fxops);

            var homeAddr = "https://www.taobao.com/";
            _driver.Navigate().GoToUrl(homeAddr);

            Console.WriteLine(_driver.PageSource);
            var monitorAddr = "https://myseller.taobao.com/home.htm";
            _driver.FindElement(By.XPath("//*[@id='J_SiteNavSeller']/div[1]/a")).SendKeys(Keys.Control + Keys.Enter);
            Thread.Sleep(3000);
            _tabs = _driver.WindowHandles.ToList();

        }



        public IWebDriver CreateFireFoxBrowser(FirefoxOptions option)
        {
            var path = "";
            IWebDriver driver = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                //path = _config["FireFoxPath_Linux"];
                path = "/home/hoyho/workspace/WeGouSharp/src/WeGouSharp/Resource/firefox_linux/firefox";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                path = _config["FireFoxPath_OSX"];
            }
            else
            {
                path = _config["FireFoxPath_Windows"];
            }
            var fds = FirefoxDriverService.CreateDefaultService();
            fds.FirefoxBinaryPath = path;
            option.AddArgument("-headless");

            driver = new FirefoxDriver(fds, option, TimeSpan.FromMinutes(1));

            return driver;
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