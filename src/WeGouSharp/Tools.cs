using OpenCvSharp;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeGouSharp
{
    class Tools
    {
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
                Mat matImg = Mat.FromImageData(bytes);
                Cv2.Resize(matImg, matImg, new Size(260, 84)); //the dst image size,e.g.100x100

                Cv2.ImShow(windowName, matImg); //Show the image
                Cv2.WaitKey(); //no wait，when value great than 0, then wait n seco
                Cv2.DestroyWindow(windowName); //Destroy the window if key is pressed
            });
        }

        //保存验证码
        public static string SaveImage(string base64String, string imgName)
        {
            var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location); //Path
            path = Path.Combine(path ?? throw new WechatSogouFileException(), "captcha");
            //Check if directory exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            //set the image path
            string imgPath = Path.Combine(path, imgName);

            byte[] imageBytes = Convert.FromBase64String(base64String);

            File.WriteAllBytes(imgPath, imageBytes);
            return imgPath;
        }


        /// <summary>
        /// 保存文件到训练目录
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="destPath"></param>
        public static void CopytoTrain(string srcPath, string destPath)
        {
            try
            {
                var file = new FileInfo(srcPath);
                if (file.Exists)
                {
                    // true is overwrite
                    file.CopyTo(destPath, true);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                throw new WechatSogouFileException();
            }
            
        }


        /// <summary>
        /// 尝试转换对象为为json格式
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="WechatSogouJsonException"></exception>
        public static string TryParseJson(object target)
        {
            try
            {
                return JsonConvert.SerializeObject(target, Formatting.Indented);
            }
            catch
            {
                throw new WechatSogouJsonException();
            }
        }
    }
}