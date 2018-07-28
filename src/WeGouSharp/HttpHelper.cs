using log4net;
using System;
using System.IO;
using System.Net;

namespace WeGouSharp
{
    class HttpHelper
    {
        private readonly ILog _logger;

        //string _vcodeUrl = ""; //需要填验证码的url
        public HttpHelper()
        {
            _logger = ServiceProviderAccessor.ServiceProvider.GetService(typeof(ILog)) as ILog;
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
            string responseFromServer = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();

                // Cleanup the streams and the response.
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.Error((e));
                //throw;
            }

            return responseFromServer;
        }

    }


}
