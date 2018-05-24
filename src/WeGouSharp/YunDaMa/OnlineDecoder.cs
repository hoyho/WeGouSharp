using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace WeGouSharp.YunDaMa
{
    /// <summary>
    /// 云打码实现类
    /// </summary>
    public class OnlineDecoder : IDecode
    {
        private int _tryTime = 1;

        private int _maxTry = 1;

        private IConfiguration conf { get; set; }

        public OnlineDecoder(IConfiguration configuration)
        {
            this.conf = configuration;
        }
        public OnlineDecoder()
            //:this(configuration:new ServiceCollection().BuildServiceProvider().GetService<ConfigurationRoot>())
        {
            conf = Config.Configuration;
        }

        public string OnlineDecode(string imageLocation)
        {
            var userName = conf.GetSection("yundama_username").Value;
            var psw = conf.GetSection("yundama_password").Value;
            var codetype = conf.GetSection("yundama_codetype").Value;
            var appid = conf.GetSection("yundama_appid").Value;
            var appkey = conf.GetSection("yundama_appkey").Value;
            var timeout = conf.GetSection("yundama_timeout").Value;

            var uploadResult =  PostForm(userName,psw,codetype,"upload",appid,appkey,timeout,"captcha/vcode.jpg" );
            var ydm = JsonConvert.DeserializeObject<Model.YunDaMa>(uploadResult);
            if (ydm!=null && !string.IsNullOrEmpty(ydm.text))
            {
                Tools.CopytoTrain("captcha/vcode.jpg",$"trainingFiles/{ydm.text}.jpg");
                return ydm.text;//已经上传过，直接读取结果
            }

            this._tryTime = 0; //重置，下面会用到
            var decodeResult =  GetDecodeResult(ydm?.cid);
            ydm = JsonConvert.DeserializeObject<Model.YunDaMa>(decodeResult);
            Tools.CopytoTrain("captcha/vcode.jpg",$"trainingFiles/{ydm.text}.jpg");
            return ydm.text;

        }

        public void SetTryLimit(int max)
        {
            _maxTry = max > 0 ? max : 1;
        }

        /// <summary>
        /// 上传图片返回cid
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="codeType"></param>
        /// <param name="action"></param>
        /// <param name="appid"></param>
        /// <param name="appkey"></param>
        /// <param name="timeOut"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string PostForm(string userName, string passWord, string codeType, string action, string appid,
            string appkey, string timeOut, string filePath)
        {
            string APIUrl = "http://api.yundama.com/";

            //add post values and files
            NameValueCollection values = new NameValueCollection();
            NameValueCollection files = new NameValueCollection();
            values.Add("username", userName);
            values.Add("password", passWord);
            values.Add("codetype", codeType);
            values.Add("method", action);
            values.Add("appid", appid);
            values.Add("appkey", appkey);
            values.Add("timeout", timeOut);
            files.Add("file", filePath);
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            // The first boundary
            byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            // The last boundary
            byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            // The first time it itereates, we need to make sure it doesn't put too many new paragraphs down or it completely messes up poor webbrick
            System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            // Create the request and set parameters
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(APIUrl + "api.php");
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            try
            {
                // Get request stream
                Stream requestStream = request.GetRequestStream();
                // Write post item to stream (write boundary and form item )
                foreach (string key in values.Keys)
                {
                    byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(
                        string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", key, values[key]));
                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                    requestStream.Write(formItemBytes, 0, formItemBytes.Length);
                }

                // Write file content to stream, byte by byte
                foreach (string key in files.Keys)
                {
                    if (File.Exists(files[key]))
                    {
                        byte[] buffer = new byte[2048];
                        byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(
                            $"Content-Disposition: form-data; name=\"file\"; filename=\"{key}\"\r\nContent-Type: application/octet-stream\r\n\r\n");
                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                        requestStream.Write(formItemBytes, 0, formItemBytes.Length);
                        using (FileStream fileStream = new FileStream(files[key], FileMode.Open, FileAccess.Read))
                        {
                            var bytesRead = 0;
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                requestStream.Write(buffer, 0, bytesRead);
                            }

                            fileStream.Close();
                        }
                    }
                }

                // Write trailer and close stream
                requestStream.Write(trailer, 0, trailer.Length);
                requestStream.Close();

                using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _tryTime += 1;
                return _tryTime > _maxTry
                    ? ""
                    : PostForm(userName, passWord, codeType, action, appid,
                        appkey, timeOut, filePath);
            }
        }

        /// <summary>
        /// 获取识别结果
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        private string GetDecodeResult(string cid)
        {
            var requestUrl = $"http://api.yundama.com/api.php?cid={cid}&method=result";
            var netHelper = new HttpHelper();
            var json = netHelper.Get(requestUrl);;
            var ydm = JsonConvert.DeserializeObject<Model.YunDaMa>(json);
            if (ydm?.ret!=0 && _tryTime<_maxTry) //结果未出。继续等待
            {
                _tryTime += 1;
                Thread.Sleep(3000);
                json = GetDecodeResult(cid);
            }

            return json;
        }
        
    }
}