using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WeGouSharp
{
    class Tools
    {

       public static  string replace_space(string s)
        {
            return s.Replace(" ","").Replace("\r\n", "");
        }

        public static string replace_html(string data)
        {
            //to do 
            Dictionary<string, string> result = new Dictionary<string, string> { };
            string json = (new JavaScriptSerializer()).Serialize(result);
            return json;

    //        if isinstance(data, dict):
    //    return dict([(replace_html(k), replace_html(v)) for k, v in data.items()])
    //            elif isinstance(data, list):
    //    return [replace_html(l) for l in data]
    //    elif isinstance(data, str):
    //    return _replace_str_html(data)
    //else:
    //    return data
        }

        public static string get_elem_text(string element)
        {

            return "";
    //        """抽取lxml.etree库中elem对象中文字

    //Args:
    //    elem: lxml.etree库中elem对象

    //Returns:
    //    elem中文字
    //"""
    //return ''.join([node.strip() for node in elem.itertext()])

        }
   

        /// <summary>
        /// 调用OpenCV显示验证码
        /// </summary>
        /// <param name="base64String"></param>
        /// 
        public static void DisplayImageFromBase64(string base64String)
        {

            var bytes = Convert.FromBase64String(base64String);
            using(MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
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


        public delegate void ShowImageHandle(string base64String);



    }
}
