using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using WeGouSharp.Model;

namespace WeGouSharp.YunDaMa
{
    /// <summary>
    /// 云打码实现类
    /// </summary>
    public class OnlineDecoder : IDecoder
    {
        private int _tryTime = 1;

        private int _maxTry = 1;

        private YunDaMaConfig Conf { get; set; }
        private ILog _logger;


        public OnlineDecoder(YunDaMaConfig yConfig, ILog logger)
        {
            Conf = yConfig;
            _logger = logger;
        }

        public string Decode(string imageLocation, CaptchaType captchaType)
        {
            var userName = Conf.UserName;
            var psw = Conf.PassWord;
            var appid = Conf.AppId;
            var appkey = Conf.AppKey;
            var timeout = Conf.TimeOut;
            var codeType = ((int)captchaType).ToString();

            var codePath = "";
            var exePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location); //Path


            if (captchaType == CaptchaType.Sogou)
            {
                codePath = Path.Combine(exePath ?? throw new WechatSogouVcodeException("get vode path fail"),
                    $"captcha/{captchaType}/vcode.jpg");
            }
            else if (captchaType == CaptchaType.WeiXin)
            {
                codePath = Path.Combine(exePath ?? throw new WechatSogouVcodeException("get vode path fail"),
                    $"captcha/{captchaType}/vcode.png");
            }

            var uploadResult = RestUpload(userName, psw, codeType, "upload", appid, appkey, timeout, codePath);

            var ydm = JsonConvert.DeserializeObject<YunDaMaResponse>(uploadResult);
            if (ydm != null && !string.IsNullOrEmpty(ydm.text)) //如果已经上传过会立即得到结果
            {
                var trainingFilePath = "";
                if (captchaType == CaptchaType.Sogou)
                {
                    trainingFilePath = $"trainingFiles/{captchaType}/{ydm.text}.jpg";
                }
                else if (captchaType == CaptchaType.WeiXin)
                {
                    trainingFilePath = $"trainingFiles/{captchaType}/{ydm.text}.png";
                }

                Tools.CopytoTrain(codePath, trainingFilePath);
                return ydm.text; //已经上传过，直接读取结果
            }


            var decodeResult = GetDecodeResult(ydm?.cid);
            ydm = JsonConvert.DeserializeObject<YunDaMaResponse>(decodeResult);


            if (ydm != null && !string.IsNullOrEmpty(ydm.text))
            {
                var trainingFilePath = "";
                if (captchaType == CaptchaType.Sogou)
                {
                    trainingFilePath = $"trainingFiles/{captchaType}/{ydm.text}.jpg";
                }
                else if (captchaType == CaptchaType.WeiXin)
                {
                    trainingFilePath = $"trainingFiles/{captchaType}/{ydm.text}.png";
                }
                Tools.CopytoTrain(codePath, trainingFilePath);
            }

            return ydm?.text;
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
            NameValueCollection values = new NameValueCollection
            {
                {"username", userName},
                {"password", passWord},
                {"codetype", codeType},
                {"method", action},
                {"appid", appid},
                {"appkey", appkey},
                {"timeout", timeOut}
            };
            NameValueCollection files = new NameValueCollection { { "file", filePath } };

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            // The first boundary
            byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            // The last boundary
            byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            // The first time it itereates, we need to make sure it doesn't put too many new paragraphs down or it completely messes up poor webbrick
            System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            // Create the request and set parameters
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrl + "api.php");
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
                        $"Content-Disposition: form-data; name=\"{key}\";\r\n\r\n{values[key]}");
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
                            int bytesRead;
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

                using (StreamReader reader =
                    new StreamReader(request.GetResponse().GetResponseStream() ??
                                     throw new WechatSogouRequestException()))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                _tryTime += 1;
                return _tryTime > _maxTry
                    ? ""
                    : PostForm(userName, passWord, codeType, action, appid,
                        appkey, timeOut, filePath);
            }
        }

        public static string RestUpload(string userName, string passWord, string codeType, string action, string appid,
            string appkey, string timeOut, string filePath)
        {
            string apiUrl = "http://api.yundama.com/";
            try
            {
                var client = new RestClient(apiUrl);

                var request = new RestRequest("api.php", Method.POST);
                request.AddParameter("username", userName);
                request.AddParameter("password", passWord);
                request.AddParameter("codetype", codeType);
                request.AddParameter("method", action);
                request.AddParameter("appid", appid);
                request.AddParameter("appkey", appkey);
                request.AddParameter("timeout", timeOut);

                // add files to upload (works with compatible verbs)
                request.AddFile("file", filePath);

                // execute the request
                IRestResponse response = client.Execute(request);
                var resp = response.Content; // raw content as string
                Console.WriteLine(resp);
                return resp;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 获取识别结果
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        private string GetDecodeResult(string cid, int tryTime = 0)
        {
            int maxTry = 5;
            var requestUrl = $"http://api.yundama.com/api.php?cid={cid}&method=result";
            var netHelper = new HttpHelper();

            if (tryTime > maxTry)
            {
                Console.WriteLine("get decode result exceed max try time!!");
                throw new WechatSogouRequestException();
            }

            var json = netHelper.Get(requestUrl);
            var ydm = JsonConvert.DeserializeObject<YunDaMaResponse>(json);

            if (ydm?.ret != 0 && tryTime < maxTry) //结果未出。继续等待
            {
                tryTime += 1;
                Thread.Sleep(4000);
                json = GetDecodeResult(cid,tryTime);
                Console.WriteLine($"Code result:{json}, tryTime:{tryTime}");
            }

            return json;
        }
    }
}