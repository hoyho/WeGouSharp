using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace WeGouSharp.YunDaMa
{
    public static class PostFormData
    {
        public static string PostForm(string userName, string passWord, string codeType, string action, string appid,
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
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

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
                return "";
            }
        }
    }
}