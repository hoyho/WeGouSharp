using log4net;
using System;
using System.IO;
using System.Text;
using static WeGouSharp.Tools;
using System.Runtime.InteropServices;
using WeGouSharp.YunDaMa;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using WeGouSharp.Infrastructure;
using WeGouSharp.Model;

namespace WeGouSharp
{
    public class Browser
    {
        private readonly FirefoxDriver _driver;

        readonly IConfiguration _config;

        private ILog _logger;

        public Browser(ILog logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FirefoxProfile ffProfile = new FirefoxProfile();

            var preSettings = _config.GetSection("Driver:FireFoxPreference").Get<Dictionary<string, string>>();

            if (preSettings != null)
            {
                foreach (var keyValuePair in preSettings)
                {
                    ffProfile.SetPreference(keyValuePair.Key, keyValuePair.Value);
                    //                    ffProfile.SetPreference("browser.download.folderList", 1); //0:desktop 1:download folder 2:custom
                    //                    ffProfile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "text/plain");
                }
            }

            FirefoxOptions ffopt = new FirefoxOptions() {Profile = ffProfile, LogLevel = FirefoxDriverLogLevel.Fatal};

            _driver = LaunchFireFox(ffopt);


            var homeAddr = "http://weixin.sogou.com/";
            _driver.Navigate().GoToUrl(homeAddr);

            //Console.WriteLine(_driver.PageSource);
        }


        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        private FirefoxDriver LaunchFireFox(FirefoxOptions option)
        {
            var useEmbededBrowser = _config.GetValue<bool>("Driver:UseEmbededBrowser");

            var browserPath = useEmbededBrowser ? GetBrowserPath() : "";

            //folder containe geckodriver
            var geckodriverFullPath = GetGeckoDriverPath();
            var pathArray = geckodriverFullPath.Split(',');

            //use embeded driver and geckodriver
            var fds = string.IsNullOrEmpty(geckodriverFullPath)
                ? FirefoxDriverService.CreateDefaultService()
                : FirefoxDriverService.CreateDefaultService(pathArray[0], pathArray[1]);

            if (!string.IsNullOrWhiteSpace(browserPath))
            {
                fds.FirefoxBinaryPath = browserPath;
            }


            var args = _config.GetSection("Driver:Argument").Get<List<string>>();
            //option.AddArgument("-headless");
            if (args != null && args.Count > 0)
            {
                option.AddArguments(args);
            }

            var driver = new FirefoxDriver(fds, option, TimeSpan.FromMinutes(1));
            return driver;
        }


        /// <summary>
        /// 输入网址返回页面内容,如果有验证码则抛出异常
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="WechatSogouVcodeException"></exception>
        public Task<string> GetPageWithoutVcodeAsync(string url)
        {
            return Task.Run(() =>
            {
                _driver.Navigate().GoToUrl(url);
                var pageSrc = _driver.PageSource;
                if (pageSrc.Contains("用户您好，您的访问过于频繁，为确认本次访问为正常用户行为，需要您协助验证"))
                {
                    throw new WechatSogouVcodeException("vcode")
                    {
                        VisittingUrl = url
                    };
                }

                if (pageSrc.Contains("为了您的安全请输入验证码") && _driver.Url.Contains("mp.weixin.qq.com"))
                {
                    throw new WechatWxVcodeException("vcode")
                    {
                        VisittingUrl = url
                    };
                }

                return _driver.PageSource;
            });
        }

        /// <summary>
        /// 输入网址返回页面内容,即使包含验证码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<string> GetPageAsync(string url)
        {
            return Task.Run(() =>
            {
                _driver.Navigate().GoToUrl(url);
                return _driver.PageSource;
            });
        }


        //微信文章页出现的验证码
        public async Task HandleWxVcodeAsync(string vCodeUrl, bool useCloudDecode = true)
        {
            await GetPageAsync(vCodeUrl);

            var base64Img = _driver.ExecuteScript(@"
                var c = document.createElement('canvas');
                var ctx = c.getContext('2d');
                var img = document.getElementById('verify_img');
                c.height=img.height;
                c.width=img.width;
                ctx.drawImage(img, 0, 0,img.width, img.height);
                var base64String = c.toDataURL();
                return base64String;
                ") as string;

            base64Img = base64Img?.Replace("data:image/png;base64,", "");

            string vCodeSavePath = SaveImage(base64Img, CaptchaType.WeiXin, "vcode.png");


            string verifyCode;
            if (useCloudDecode)
            {
                var decoder = ServiceProviderAccessor.ServiceProvider.GetService(typeof(IDecoder)) as IDecoder;
                verifyCode = decoder?.Decode(vCodeSavePath, CaptchaType.WeiXin);
            }
            else
            {
                await ShowVcodeAsync(base64Img, vCodeSavePath);
                verifyCode = Console.ReadLine();
            }


            var codeInput = _driver.FindElementById("input");
            codeInput.SendKeys(verifyCode);

            _driver.FindElementById("bt").Click();
        }


        //搜狗页的验证码
        public async Task<bool> HandleSogouVcodeAsync(string vCodeUrl, bool useCloudDecode = false)
        {
            await GetPageAsync(vCodeUrl);

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

            base64Img = base64Img?.Replace("data:image/png;base64,", "");

            string vCodeSavePath = SaveImage(base64Img, CaptchaType.Sogou, "vcode.jpg");
            ;

            string verifyCode;
            if (useCloudDecode)
            {
                var decoder = ServiceProviderAccessor.ServiceProvider.GetService(typeof(IDecoder)) as IDecoder;
                verifyCode = decoder?.Decode(vCodeSavePath, CaptchaType.Sogou);
            }
            else
            {
                await ShowVcodeAsync(base64Img, vCodeSavePath);
                verifyCode = Console.ReadLine();
            }


            var codeInput = _driver.FindElementById("seccodeInput");
            codeInput.SendKeys(verifyCode);

            _driver.FindElementById("submit").Click();
            return true;
        }


        private void SetFirefoxProxy()
        {
            _driver.Navigate().GoToUrl("http://2018.ip138.com/ic.asp");
            var pxHost = "127.0.0.1";
            var pxPort = "8001";

            _driver.Navigate().GoToUrl("about:config");
            var setupScript =
                @"var prefs = Components.classes[""@mozilla.org/preferences-service;1""].getService(Components.interfaces.nsIPrefBranch);
                prefs.setIntPref(""network.proxy.type"", 1);
                prefs.setCharPref(""network.proxy.http"", ""127.0.0.1"");
                prefs.setIntPref(""network.proxy.http_port"", ""8001"");
                  ";
                //prefs.setCharPref(""network.proxy.ssl"", ""127.0.0.1"");
                //prefs.setIntPref(""network.proxy.ssl_port"", ""8001"");
                //prefs.setCharPref(""network.proxy.ftp"", ""127.0.0.1"");
                //prefs.setIntPref(""network.proxy.ftp_port"", ""8001"");
                //prefs.setCharPref(""network.proxy.socks"", ""127.0.0.1"");
                //prefs.setIntPref(""network.proxy.socks_port"", ""8001"");
            
            //running script below  
            _driver.ExecuteScript(setupScript);

            //sleep for 1 secs
            System.Threading.Thread.Sleep(1000);
            //_driver.Navigate().GoToUrl("http://2018.ip138.com/ic.asp");
        }

        /// <summary>
        /// 运行shell判断系统是否有Desktop 环境
        /// </summary>
        /// <returns></returns>
        private bool IsRunWithXServer()
        {
            var cmd = @"
            if xhost >& /dev/null ; 
            then echo 'True'
            else echo 'False' ;
            fi";
            var rs = cmd.RunAsShell().Replace("\n", "");

            if (rs.Trim().ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private string GetBrowserPath()
        {
            var browserPath = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                && IsRunWithXServer()
            ) //linux with desktop environemnt
            {
                browserPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource/firefox_linux/firefox");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                browserPath = "/Applications/Firefox.app/Contents/MacOS/firefox";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                browserPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"Resource\firefox_windows\firefox.exe");
            }

            return browserPath;
        }


        //Return geckodriverPath and filename,seperated by comma ,
        private string GetGeckoDriverPath()
        {
            var geckodriverPath = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                geckodriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource/geckodriver/linux/")
                                  + "," + "geckodriver.sh";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                geckodriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource/geckodriver/mac/")
                                  + "," + "geckodriver.sh";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                geckodriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resource\geckodriver\windows\")
                                  + "," + "geckodriver.exe";
            }

            return geckodriverPath;
        }
    }
}