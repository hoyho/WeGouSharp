using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using static WeGouSharpPlus.Tools;

namespace WeGouSharpPlus
{
    class HttpHelper
    {

        ILog logger = LogManager.GetLogger(typeof(Program));
        string _vcode_url = ""; //需要填验证码的url




        /// <summary>
        /// 指定header参数的HTTP Get方法
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="url"></param>
        /// <returns>respondse</returns>
        public string Get(WebHeaderCollection headers, string url, string responseEncoding = "UTF-8", bool isUseCookie = false)
        {

            string responseText = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);


                request.Method = "GET";
                //request.Headers = headers;
                foreach (string key in headers.Keys)
                {
                    switch (key.ToLower())
                    {
                        case "user-agent":
                            request.UserAgent = headers[key];
                            break;
                        case "referer":
                            request.Referer = headers[key];
                            break;
                        case "host":
                            request.Host = headers[key];
                            break;
                        case "contenttype":
                            request.ContentType = headers[key];
                            break;
                        case "accept":
                            request.Accept = headers[key];
                            break;
                        default:
                            break;
                    }

                }

                if (string.IsNullOrEmpty(request.Referer))
                {
                    request.Referer = "http://weixin.sogou.com/";
                };
                if (string.IsNullOrEmpty(request.Host))
                {
                    request.Host = "weixin.sogou.com";
                };
                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    Random r = new Random();
                    int index = r.Next(WechatSogouBasic._agent.Count - 1);
                    request.UserAgent = WechatSogouBasic._agent[index];
                }
                if (isUseCookie)
                {
                    CookieCollection cc = Tools.LoadCookieFromCache();
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cc);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (isUseCookie && response.Cookies.Count > 0)
                {
                    var cookieCollection = response.Cookies;
                    WechatCache cache = new WechatCache(Config.CacheDir, 3000);
                    if (!cache.Add("cookieCollection", cookieCollection, 3000)) { cache.Update("cookieCollection", cookieCollection, 3000); };
                }
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();




                //如果response是图片，则返回以base64方式返回图片内容，否则返回html内容
                if (response.Headers.Get("Content-Type") == "image/jpeg" || response.Headers.Get("Content-Type") == "image/jpg")
                {
                 
                    //totest
                    Image img = Image.FromStream(dataStream, true);

                    using (MemoryStream ms = new MemoryStream())
                    {

                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] imageBytes = ms.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        responseText = base64String;
                    }

                }
                else //read response string
                {

                    // Open the stream using a StreamReader for easy access.
                    Encoding encoding;
                    switch (responseEncoding.ToLower())
                    {
                        case "utf-8":
                            encoding = Encoding.UTF8;
                            break;
                        case "unicode":
                            encoding = Encoding.Unicode;
                            break;
                        case "ascii":
                            encoding = Encoding.ASCII;
                            break;
                        default:
                            encoding = Encoding.Default;
                            break;

                    }
                    StreamReader reader = new StreamReader(dataStream, encoding);//System.Text.Encoding.Default
                    // Read the content.

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        responseText = reader.ReadToEnd();
                        if (responseText.Contains("用户您好，您的访问过于频繁，为确认本次访问为正常用户行为，需要您协助验证"))
                        {
                            _vcode_url = url;
                            throw new Exception("weixin.sogou.com verification code");
                        }
                    }
                    else
                    {
                        logger.Error("requests status_code error" + response.StatusCode);
                        throw new Exception("requests status_code error");
                    }
                    reader.Close();

                }


                // Cleanup the streams and the response.
                dataStream.Close();
                response.Close();


            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return responseText;
        }




        /// <summary>
        /// 简单的HTTP GET方法
        /// </summary>
        /// <param name="url"></param>
        /// <returns>response</returns>
        public string Get(string url)
        {


            var request = (HttpWebRequest)WebRequest.Create(url);


            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }






        /// <summary>
        /// Post请求， body是json类型的数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="jsonBody"></param>
        /// <returns></returns>
        public string PostJson(string url, WebHeaderCollection headers, string jsonBody)
        {
            string responseText = "";
            try
            {

                var request = (HttpWebRequest)WebRequest.Create(url);


                foreach (string key in headers.Keys)
                {
                    switch (key.ToLower())
                    {
                        case "user-agent":
                            request.UserAgent = headers[key];
                            break;
                        case "referer":
                            request.Referer = headers[key];
                            break;
                        case "host":
                            request.Host = headers[key];
                            break;
                        case "contenttype":
                            request.ContentType = headers[key];
                            break;
                        default:
                            break;
                    }

                }

                if (string.IsNullOrEmpty(request.Referer))
                {
                    request.Referer = "http://weixin.sogou.com/";
                };
                if (string.IsNullOrEmpty(request.Host))
                {
                    request.Host = "weixin.sogou.com";
                };
                // request.Headers.Add("Token", token);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                System.Text.Encoding encoding = Encoding.UTF8;
                byte[] buffer = encoding.GetBytes(jsonBody);
                request.ContentLength = buffer.Length;

                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseText = reader.ReadToEnd();
                        if (responseText.Contains(""))
                        {
                            _vcode_url = url;
                        }
                    }
                }
                else
                {
                    logger.Error("requests status_code error" + response.StatusCode);
                    throw new Exception("requests status_code error");
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return responseText;
        }



        /// <summary>
        /// 简单HTTP POST方法,用于post验证码，Content-Type: application/x-www-form-urlencoded
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Post(string url, WebHeaderCollection headers, string postData, bool isUseCookie = false)
        {

            string responseText = "";
            try
            {

                var request = (HttpWebRequest)WebRequest.Create(url);


                foreach (string key in headers.Keys)
                {
                    switch (key.ToLower())
                    {
                        case "user-agent":
                            request.UserAgent = headers[key];
                            break;
                        case "referer":
                            request.Referer = headers[key];
                            break;
                        case "host":
                            request.Host = headers[key];
                            break;
                        case "contenttype":
                            request.ContentType = headers[key];
                            break;
                        default:
                            break;
                    }

                }

                if (string.IsNullOrEmpty(request.Referer))
                {
                    request.Referer = "http://weixin.sogou.com/";
                };
                if (string.IsNullOrEmpty(request.Host))
                {
                    request.Host = "weixin.sogou.com";
                };

                if (isUseCookie)
                {
                    request.CookieContainer = new CookieContainer();
                    CookieCollection cc = Tools.LoadCookieFromCache();
                    request.CookieContainer.Add(cc);
                }



                request.Method = "POST";

                request.ContentType = "application/x-www-form-urlencoded";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length;

                // Get the request stream.  
                Stream inDataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                inDataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                inDataStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();


                if (isUseCookie && response.Cookies.Count > 0)
                {
                    var cookieCollection = response.Cookies;
                    WechatCache cache = new WechatCache(Config.CacheDir, 3000);
                    if (!cache.Add("cookieCollection", cookieCollection, 3000)) { cache.Update("cookieCollection", cookieCollection, 3000); };
                }

                // Get the stream containing content returned by the server.
                Stream outDataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(outDataStream);
                // Read the content.
                responseText = reader.ReadToEnd();

                // Cleanup the streams and the response.
                reader.Close();
                outDataStream.Close();
                response.Close();


            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return responseText;
        }











        /// <summary>
        /// 对于出现验证码，识别验证码，输入验证码解封
        /// </summary>
        /// <returns></returns>
        public bool UnLock(bool isOCR)
        {
            logger.Debug("vcode appear, use UnLock()");
            string codeurl = "http://weixin.sogou.com/antispider/util/seccode.php?tc=" + DateTime.Now.Ticks;
            //codeurl = 'http://weixin.sogou.com/antispider/util/seccode.php?tc=' + str(time.time())[0:10]
            HttpHelper netHelper = new HttpHelper();
            WebHeaderCollection headers = new WebHeaderCollection();
            var content = netHelper.Get(headers, codeurl);

            //异步显示验证码
            ShowImageHandle showImageHandle = new ShowImageHandle(DisplayImageFromBase64);
            showImageHandle.BeginInvoke(content, null, null);

            if (isOCR)
            {
                //todo
            }
            else
            {

            }



            Console.WriteLine("请输入验证码：");
            string verifyCode = Console.ReadLine();
            string postURL = "http://weixin.sogou.com/antispider/thank.php";
            string postData = string.Format("{'c': {0}, 'r': {1}, 'v': 5 }", verifyCode, this._vcode_url);
            Random r = new Random();
            int index = r.Next(WechatSogouBasic._agent.Count - 1);
            headers.Add("User-Agent", WechatSogouBasic._agent[index]);
            headers.Add("Referer", "http://weixin.sogou.com/antispider/?from=%2" + this._vcode_url.Replace("http://", ""));
            headers.Add("Host", "weixin.sogou.com");
            string remsg = netHelper.PostJson(postURL, headers, postData);
            JObject jo = JObject.Parse(remsg);//把json字符串转化为json对象  
            int satuscode = (int)jo.GetValue("code");

            if (satuscode < 0)
            {
                logger.Error("cannot unblock because " + jo.GetValue("msg"));
                var vcodeException = new WechatSogouVcodeException();
                vcodeException.MoreInfo = "cannot jiefeng because " + jo.GetValue("msg");
                throw vcodeException;
            }
            else
            {
                //this._cache.set(Config.CacheDir, "", 500);
                Console.WriteLine("ocr");
                return true;
            }


        }




        /// <summary>
        /// 对于出现频率限制验证码，识别验证码，输入验证码解封
        /// </summary>
        /// <returns></returns>
        /// <comment>原_jiefeng()</comment>
        public bool UnblockFrequencyLimit(string balckUrl, bool isOCR)
        {
            logger.Debug("vcode appear, use UnLock()");
            string codeurl = "http://weixin.sogou.com/antispider/util/seccode.php?tc=" + DateTime.Now.Ticks;
            //codeurl = 'http://weixin.sogou.com/antispider/util/seccode.php?tc=' + str(time.time())[0:10]
            HttpHelper netHelper = new HttpHelper();
            WebHeaderCollection headers = new WebHeaderCollection();
            var content = netHelper.Get(headers, codeurl);

            //异步显示验证码
            ShowImageHandle showImageHandle = new ShowImageHandle(DisplayImageFromBase64);
            showImageHandle.BeginInvoke(content, null, null);
            Console.WriteLine("请输入验证码：");
            string verifyCode = Console.ReadLine();
            string postURL = "http://weixin.sogou.com/antispider/thank.php";
            string postData = "{" + string.Format(@"'c':'{0}','r':'{1}','v': 5", verifyCode, balckUrl) + "}";//{'c': '{0}', 'r': '{1}', 'v': 5 }
            Random r = new Random();
            int index = r.Next(WechatSogouBasic._agent.Count - 1);
            headers.Add("User-Agent", WechatSogouBasic._agent[index]);
            headers.Add("Referer", "http://weixin.sogou.com/antispider/?from=%2" + balckUrl);
            headers.Add("Host", "weixin.sogou.com");
            string remsg = netHelper.PostJson(postURL, headers, postData);
            JObject jo = JObject.Parse(remsg);//把json字符串转化为json对象  
            int satuscode = (int)jo.GetValue("code");

            if (satuscode < 0)
            {
                logger.Error("cannot unblock because " + jo.GetValue("msg"));
                var vcodeException = new WechatSogouVcodeException();
                vcodeException.MoreInfo = "cannot jiefeng because " + jo.GetValue("msg");
                throw vcodeException;
            }
            else
            {
                Console.WriteLine("ocr");
                return true;
            }


        }


        /// <summary>
        /// 页面出现验证码，输入才能继续,此验证依赖cookie, 获取验证码的requset有个cookie，每次不同，需要在post验证码的时候带上
        /// </summary>
        /// <returns></returns>
        public bool VerifyCodeForContinute(string url, bool isUseOCR)
        {
            bool isSuccess = false;
            logger.Debug("vcode appear, use VerifyCodeForContinute()");
            DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timeStamp17 = (DateTime.UtcNow - Epoch).TotalMilliseconds.ToString("R"); //get timestamp with 17 bit
            string codeurl = "https://mp.weixin.qq.com/mp/verifycode?cert=" + timeStamp17;
            WebHeaderCollection headers = new WebHeaderCollection();
            var content = this.Get(headers, codeurl, "UTF-8", true);
            // ShowImageHandle showImageHandle = new ShowImageHandle(DisplayImageFromBase64);
            // showImageHandle.BeginInvoke(content, null, null);
            DisplayImageFromBase64Async(content);
            Console.WriteLine("\n请输入验证码：");
            string verifyCode = Console.ReadLine();
            string postURL = "https://mp.weixin.qq.com/mp/verifycode";

            timeStamp17 = (DateTime.UtcNow - Epoch).TotalMilliseconds.ToString("R"); //get timestamp with 17 bit
            string postData = string.Format("cert={0}&input={1}", timeStamp17, verifyCode);// "{" + string.Format(@"'cert':'{0}','input':'{1}'", timeStamp17, verifyCode) + "}";
            headers.Add("Host", "mp.weixin.qq.com");
            headers.Add("Referer", url);
            string remsg = this.Post(postURL, headers, postData, true);

            try
            {

                JObject jo = JObject.Parse(remsg);//把json字符串转化为json对象  
                int statusCode = (int)jo.GetValue("ret");

                if (statusCode == 0)
                {
                    isSuccess = true;
                }
                else
                {
                    logger.Error("cannot unblock because " + jo.GetValue("msg"));
                    var vcodeException = new WechatSogouVcodeException();
                    vcodeException.MoreInfo = "cannot jiefeng because " + jo.GetValue("msg");
                    throw vcodeException;
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }


            return isSuccess;
        }

    }


}
