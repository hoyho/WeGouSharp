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
        //private ChromeDriver _driver;
        //private PhantomJSDriver _driver;


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

            var cOption = new ChromeOptions()
            {

            };
            _driver = (FirefoxDriver)CreateFireFoxBrowser(fxops);
            var pOpt = new PhantomJSOptions();
          //  _driver = (PhantomJSDriver)CreatePhanton(pOpt);

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
            var browserPath = "";
            var geckodriverPath = "/tmp/";//todo

            IWebDriver driver = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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

            //option.SetPreference("webdriver.gecko.driver", @"/tmp/geckodriver");

            driver = new FirefoxDriver(fds, option, TimeSpan.FromMinutes(1));

            return driver;
        }



        public IWebDriver CreatePhanton(PhantomJSOptions option)
        {
            var browserPath = "";
            var geckodriverPath = "/home/hoyho/workspace/WeGouSharp/src/WeGouSharp/Resource/phantomjs-2.1.1-linux-x86_64/";
            //todo

            IWebDriver driver = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
            var pds = PhantomJSDriverService.CreateDefaultService(geckodriverPath);
            //option.AddArgument("-headless");

            driver = new PhantomJSDriver(pds, option, TimeSpan.FromMinutes(1));

            return driver;
        }

        public IWebDriver CreateChrome(ChromeOptions option)
        {
            var browserPath = "";
            var driverPath = "/home/hoyho/workspace/WeGouSharp/src/WeGouSharp/Resource/chromedriver_linux64";//todo

            IWebDriver driver = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
            var cds = ChromeDriverService.CreateDefaultService(driverPath);
            //option.AddArgument("headless");

            driver = new ChromeDriver(cds, option, TimeSpan.FromMinutes(1));

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