using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WeGouSharpPlus
{
    class Tools
    {

        /// <summary>
        /// 调用OpenCV显示验证码
        /// </summary>
        /// <param name="base64String"></param>
        /// 
        public static void DisplayImageFromBase64(string base64String)
        {

            var bytes = Convert.FromBase64String(base64String);
            //totest
            //Emgucv is incompatible with dotnet core try to use OpenCVSharp
            string windowName = "Your Captcha"; //The name of the window
            Cv2.NamedWindow(windowName); //Create the window using the specific name
            Mat matImg = Mat.FromImageData(bytes, ImreadModes.Color);
            Cv2.Resize(matImg, matImg, new OpenCvSharp.Size(260, 84)); //the dst image size,e.g.100x100

            Cv2.ImShow(windowName, matImg); //Show the image
            Cv2.WaitKey(0);  //no wait
            Cv2.DestroyWindow(windowName); //Destroy the window if key is pressed


        }

        //异步显示验证码图片
        public static Task DisplayImageFromBase64Async(string base64String)
        {
            return Task.Factory.StartNew(() =>
             {
                 var bytes = Convert.FromBase64String(base64String);
                 //totest
                 //Emgucv is incompatible with dotnet core try to use OpenCVSharp
                 string windowName = "Your Captcha"; //The name of the window
                 Cv2.NamedWindow(windowName); //Create the window using the specific name
                 Mat matImg = Mat.FromImageData(bytes, ImreadModes.Color);
                 Cv2.Resize(matImg, matImg, new OpenCvSharp.Size(260, 84)); //the dst image size,e.g.100x100

                 Cv2.ImShow(windowName, matImg); //Show the image
                 Cv2.WaitKey(0);  //no wait，when value great than 0, then wait n seco
                 Cv2.DestroyWindow(windowName); //Destroy the window if key is pressed
             });

        }

        //委托显示图片
        public delegate void ShowImageHandle(string base64String);


//保存验证码
        public static bool SaveImage(string base64String, string ImgName)
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location ); //Path
            path = Path.Combine(path,"captcha");
            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            //set the image path
            string imgPath = Path.Combine(path,ImgName);

            byte[] imageBytes = Convert.FromBase64String(base64String);

            File.WriteAllBytes(imgPath, imageBytes);

            return true;
        }


        /// <summary>
        /// 保存文件到训练目录
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="descPath"></param>
        public static void CopytoTrain(string srcPath, string descPath)
        {
            FileInfo file = new FileInfo(srcPath);
            if (file.Exists)
            {
                // true is overwrite
                file.CopyTo(descPath, true);
            }
        }
        

        static public CookieCollection LoadCookieFromCache()
        {
            WechatCache cache = new WechatCache(Config.CacheDir, 1);
            CookieCollection cc = cache.Get<CookieCollection>("cookieCollection");
            if (cc == null)
            {
                cc = new CookieCollection();
            }

            return cc;
        }



        public static string replaceSpace(string s)
        {
            return s.Replace(" ", "").Replace("\r\n", "");
        }






    }

}
