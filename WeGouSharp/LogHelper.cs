using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net.Repository;

namespace WeGouSharpPlus
{
    class LogHelper
    {

        //log4net日志
        public static ILog logger = LogManager.GetLogger(typeof(LogHelper));


        //月份日志
        public static void LogText(string content)
        {
            try
            {
                //System.IO.File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location  + DateTime.Now.Year + "-" + DateTime.Now.Month + ".log" , DateTime.Now + ", " +log +Environment.NewLine);
                string folder = "";
                folder = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string logFile = "";
                logFile = folder + DateTime.Now.Year + "-" + DateTime.Now.Month + ".log";
                System.IO.File.AppendAllText(logFile, DateTime.Now + ", " + content + Environment.NewLine);

            }
            catch (Exception e)
            {
                System.Console.WriteLine("LogText()-->" + e.ToString() + "\n");
            }
        }


        /// <summary>
        /// 按照每日和type进行打log
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        public static void LogDayText(string type, string content)
        {
            try
            {
                //System.IO.File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location  + DateTime.Now.Year + "-" + DateTime.Now.Month + ".log" , DateTime.Now + ", " +log +Environment.NewLine);
                string folder = "";
                folder = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string logFile = "";
                logFile = folder + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + type + ".log";
                System.IO.File.AppendAllText(logFile, DateTime.Now + ", " + content + Environment.NewLine);

            }
            catch (Exception e)
            {
                System.Console.WriteLine("LogText()-->" + e.ToString() + "\n");
            }
        }

    }
}
