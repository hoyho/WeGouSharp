using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using log4net;

namespace WeGouSharp
{

   class FileSystemCache
    {
        readonly string _path;
        int _threshold;
        readonly ILog _logger ;

        //used for temporary files by the FileSystemCache
        static string _fs_transaction_suffix = ".__wz_cache";

        public FileSystemCache(string cacheDirectory, int threshold = 500, int defaultTimeout = 300)
        {
            _path = cacheDirectory;
            _threshold = threshold;
            _logger = LogHelper.logger;
            if (!Directory.Exists(cacheDirectory))
            {
                Directory.CreateDirectory(cacheDirectory);
            }
        }


        public int _NormalizeTimeout(int timeout)
        {

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
            if (timeout != 0)
            {
                timeout = (int)timeStamp + timeout;

            }
            return timeout;
        }


        /// <summary>
        /// 列举cache文件路径
        /// </summary>
        /// <returns></returns>
        public List<string> ListCache()
        {        

            List<string> cacheList = new List<string>();
            foreach (var fileName in Directory.EnumerateFiles(_path))
            {
                if (!fileName.EndsWith(_fs_transaction_suffix))
                {
                    cacheList.Add(fileName);
                }
            }
            return cacheList;
        }


        /// <summary>
        /// 移除所有缓存
        /// </summary>
        /// <returns></returns>
        public bool _Clear()
        {
            foreach (string fname in ListCache())
            {
                try
                {
                    File.Delete(fname);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    return false;
                }

            }

            return true;
        }



        /// <summary>
        /// 根据key的utf编码并进行md运算后返回目录+文件名
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string _GetFileName(string key)
        {
            byte[] bytes = Encoding.Default.GetBytes(key.Trim());
            key = Encoding.UTF8.GetString(bytes);



            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(key);

            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {

                sb.Append(hash[i].ToString());

            }

            var fullFileName = Path.Combine(_path, sb.ToString());
            return fullFileName;
        }




        public T _Get<T>(string key) where T : new()
        {
            string fileName = _GetFileName(key);
            return _DeSerializeFromBin<T>(fileName);
        }


        public bool _Add(string key, object value, int timeout)
        {
            string fileName = _GetFileName(key);
            if (!File.Exists(fileName))
            {
                timeout = _NormalizeTimeout(timeout);
                fileName = _GetFileName(key);
                _SerializeToBin(value, fileName);
                return true;
            }
            else //已存在,应使用update/set更新
            {
                return false;
                //File.Delete(filename);
                //return this.set(key, value, timeout);;
            }


        }



        public bool _Update(string key, object value, int timeout)
        {
            string fileName = _GetFileName(key);
            if (!File.Exists(fileName))
            {
                return false;
            }
            else //已存在
            {
                timeout = _NormalizeTimeout(timeout);
                fileName = _GetFileName(key);
                _SerializeToBin(value, fileName);
                return true;
            }


        }


        
        public bool _Delete(string key)
        {
            string fileName = _GetFileName(key);

            try
            {
                File.Delete(fileName);
            }
            catch (Exception e)
            {
                _logger.Warn(e);
            }

            return false;
        }


        public bool _Has(string key)
        {
            bool isExist = false;
            string filename = _GetFileName(key);
            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Length > 0)
                {
                    isExist = true;
                }
            }

            return isExist;
        }



        /// <summary>
        /// 将一个object 系列化存入cacheFileName中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cacheFileName"></param>
        private static void _SerializeToBin(object obj, string cacheFileName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(cacheFileName));
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (FileStream fs = new FileStream(cacheFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                bf.Serialize(fs, obj);
            }

            var result = Path.ChangeExtension(cacheFileName, _fs_transaction_suffix);
        }


        /// <summary>
        /// 将缓存文件系列化为T（此处T为字典类型）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheFileName"></param>
        /// <returns></returns>
        private static T _DeSerializeFromBin<T>(string cacheFileName) where T : new()
        {
            if (File.Exists(cacheFileName))
            {
                T ret;
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read))
                {
                    ret = (T)bf.Deserialize(fs);
                }
                return ret;
            }
            
            return default(T); //返回null;

        }



    }
}