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
using log4net.Core;

namespace WeGouSharp
{
    public class Browser
    {

        private FirefoxDriver _driver;

        private List<string> _tabs = new List<string>();

        IConfiguration _config;

        ILog _logger;
        public Browser(ILog logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FirefoxProfile ffProfile = new FirefoxProfile();
            ffProfile.SetPreference("browser.download.folderList", 1); //0:desktop 1:download folder 2:custom
            ffProfile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "text/plain");

            FirefoxOptions ffopt = new FirefoxOptions() { Profile = ffProfile };
            _driver = (FirefoxDriver)LaunchFireFox(ffopt);

            var homeAddr = "http://weixin.sogou.com/";
            _driver.Navigate().GoToUrl(homeAddr);

            Console.WriteLine(_driver.PageSource);
            _tabs = _driver.WindowHandles.ToList();

        }



        public FirefoxDriver LaunchFireFox(FirefoxOptions option)
        {
            var browserPath = "";
            //folder containe geckodriver            
            var geckodriverPath = "/home/hoyho/workspace/WeGouSharp/src/WeGouSharp/Resource/firefox_linux/";

            FirefoxDriver driver = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && IsRunWithXServer()) //linux and have desktop
            {
                geckodriverPath = "Resource/firefox_linux/";
                browserPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource/firefox_linux/firefox");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                browserPath = _config["FireFoxPath_OSX"];
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                geckodriverPath = "";
                browserPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource/firefox_windows/firefox.exe");
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
                 var pageSrc = _driver.PageSource;
                 if (pageSrc.Contains("用户您好，您的访问过于频繁，为确认本次访问为正常用户行为，需要您协助验证"))
                 {

                     throw new WechatSogouVcodeException("vcode")
                     {
                         VisittingUrl = _driver.Url
                     };
                 }
                 return _driver.PageSource;
             });

        }


        //微信文章页出现的验证码
        public async Task HandleWxVcodeAsync(string vCodeUrl, bool useCloudDecode = true)
        {

            var vcodePage = await GetAsync(vCodeUrl);

            var base64Img = _driver.ExecuteScript(@"
    var c = document.createElement('canvas');
    var ctx = c.getContext('2d');
    var img = document.getElementById('seccodeImage');
    c.height=img.height;
    c.width=img.width;
    ctx.drawImage(img, 0, 0,img.width, img.height);
    var base64String = c.toDataURL();
    return base64String;
    ") as string;


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("请输入验证码：   ");
                await DisplayImageFromBase64Async(base64Img);
            }
            else
            {
                Console.WriteLine(@"your system is not support showing image in console, please open captcha from ./captcha/vcode");
                Tools.SaveImage(base64Img, "vcode.jpg");
                Console.WriteLine("请输入验证码：");
            }

            var verifyCode = "";
            if (useCloudDecode)
            {
                var decoder = ServiceProviderAccessor.ServiceProvider.GetService(typeof(IDecode)) as IDecode;
                verifyCode = decoder.OnlineDecode("chaptcha/vcode.jpg");
            }
            else
            {
                verifyCode = Console.ReadLine();
            }


            var codeInput = _driver.FindElementById("seccodeInput");
            codeInput.SendKeys(verifyCode);

            _driver.FindElementById("submit").Click();


        }


        //搜狗页的验证码
        public Task HandleSogouVcode(string vCodeUrl)
        {
            return Task.CompletedTask;
        }


        private bool IsRunWithXServer()
        {
            var cmd = @"
            if xhost >& /dev/null ; 
            then echo 'True'
            else echo 'False' ;
            fi";
            var rs = cmd.RunAsShell().Replace("\n","");

            if (rs.Trim().ToLower() == "true")
            {
                return true;
            }
            else { return false; }
        }

    }
}