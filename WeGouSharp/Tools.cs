using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WeGouSharp
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
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                // Convert byte[] to Image
                ms.Write(bytes, 0, bytes.Length);
                Bitmap bmpImage = new Bitmap(ms);
                Image<Bgr, Byte> myImage = new Image<Bgr, Byte>(bmpImage);



                String windowName = "Your Captcha"; //The name of the window
                CvInvoke.NamedWindow(windowName); //Create the window using the specific name
                Mat img = myImage.Mat;
                CvInvoke.Resize(img, img, new Size(260, 84)); //the dst image size,e.g.100x100

                //img.SetTo(new Bgr(255, 0, 0).MCvScalar); // set it to Blue color

                //Draw "Hello, world." on the image using the specific font
                //CvInvoke.PutText(myImage, "你的验证码", new System.Drawing.Point(100, 30), FontFace.HersheyComplex, 1.0, new Bgr(0, 255, 0).MCvScalar);


                CvInvoke.Imshow(windowName, img); //Show the image
                //CvInvoke.WaitKey(0);  //Wait for the key pressing event
                CvInvoke.WaitKey(0);  //no wait
                CvInvoke.DestroyWindow(windowName); //Destroy the window if key is pressed

            }





        }


        //委托显示图片
        public delegate void ShowImageHandle(string base64String);



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



        public static  string replaceSpace(string s)
        {
            return s.Replace(" ","").Replace("\r\n", "");
        }


   



    }
}
