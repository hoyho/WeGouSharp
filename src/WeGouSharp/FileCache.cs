using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WeGouSharpPlus
{
    class FileCache
    {
    }



    class WechatCache
    {
        FileSystemCache _FileCache;

        public WechatCache(string CacheDir, int DefaultTimeOut)
        {
            //cache_dir是缓存目录
            this._FileCache = new FileSystemCache(CacheDir, 500, DefaultTimeOut);
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        /// <returns></returns>
        public bool ClearAll()
        {
            return this._FileCache._Clear();
        }


        /// <summary>
        /// 根据键获取缓存内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) where T : new()
        {
            //获取键值key的缓存值
            //如果没有对应缓存，返回None
            return this._FileCache._Get<T>(key);
        }



        /// <summary>
        /// 添加一个文件缓存,如果键值key对应的缓存不存在，那么增加值value缓存，，否则返回false；
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOut">暂时未实现</param>
        /// <returns></returns>
        public bool Add(string key, object value, int timeOut)
        {
            return this._FileCache._Add(key, value, timeOut);
        }


        /// <summary>
        /// 更新cache内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOut">暂时未实现</param>
        /// <returns></returns>
        public bool Update(string key, object value, int timeOut)
        {
            return _FileCache._Update(key, value, timeOut);
        }


        /// <summary>
        /// 添加一个文件缓存,如果键值key对应的缓存不存在，那么增加值value缓存，，否则返回false；
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOut">暂时未实现</param>
        /// <returns></returns>
        public bool Has(string key)
        {
            return this._FileCache._Has(key);
        }


        /// <summary>
        /// 删除一个缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            //删除缓存
            //删除键值key存储的缓存
            return this._FileCache._Delete(key);
        }
    }

















    class FileSystemCache
    {
        string _path;
        int _threshold;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));

        //used for temporary files by the FileSystemCache
        static string _fs_transaction_suffix = ".__wz_cache";

        public FileSystemCache(string cacheDirectory, int threshold = 500, int defaultTimeout = 300)
        {
            //BaseCache.__init__(self, default_timeout)
            this._path = cacheDirectory;
            this._threshold = threshold;
            try
            {
                if (!Directory.Exists(cacheDirectory))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }

            }
            catch (Exception e)
            {
                throw e;
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

        {        //return a list of (fully qualified) cache filenames


            List<string> cacheList = new List<string> { };
            foreach (var fileName in Directory.EnumerateFiles(this._path))
            {
                if (!fileName.EndsWith(_fs_transaction_suffix))
                {
                    cacheList.Add(fileName);
                }
            }
            return cacheList;
        }

        public void _prune()
        {
            //    entries = self._list_dir()
            //if len(entries) > self._threshold:
            //    now = time()
            //    for idx, fname in enumerate(entries):
            //        try:
            //            remove = False
            //            with open(fname, 'rb') as f:
            //                expires = pickle.load(f)
            //            remove = (expires != 0 and expires <= now) or idx % 3 == 0

            //            if remove:
            //                os.remove(fname)
            //        except(IOError, OSError):
            //            pass
        }


        /// <summary>
        /// 移除所有缓存
        /// </summary>
        /// <returns></returns>
        public bool _Clear()
        {
            foreach (string fname in this.ListCache())
            {
                try
                {
                    File.Delete(fname);
                }
                catch (Exception e)
                {
                    logger.Error(e);
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
        public string _GetFileName(string key)
        {
            byte[] bytes = Encoding.Default.GetBytes(key.Trim());
            key = Encoding.UTF8.GetString(bytes);



            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(key);

            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString());

            }

            var fullFileName = Path.Combine(this._path, sb.ToString());
            return fullFileName;
        }




        public T _Get<T>(string key) where T : new()
        {
            string fileName = this._GetFileName(key);
            return (T)_DeSerializeFromBin<T>(fileName);
        }


        public bool _Add(string key, object value, int timeout)
        {
            string fileName = this._GetFileName(key);
            if (!File.Exists(fileName))
            {
                timeout = this._NormalizeTimeout(timeout);
                fileName = this._GetFileName(key);
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
            string fileName = this._GetFileName(key);
            if (!File.Exists(fileName))
            {
                return false;
            }
            else //已存在
            {
                timeout = this._NormalizeTimeout(timeout);
                fileName = this._GetFileName(key);
                _SerializeToBin(value, fileName);
                return true;
            }


        }


        //public bool _Set(string key, object value, int timeout)
        //{
        //    timeout = this._NormalizeTimeout(timeout);
        //    string fileName = this._GetFileName(key);
        //    _SerializeToBin(value, fileName);
        //    this._prune();
        //    return true;

        //}



        public bool _Delete(string key)
        {
            string fileName = this._GetFileName(key);

            try
            {
                File.Delete(fileName);
            }
            catch (Exception e)
            {
                logger.Warn(e);
            }

            return false;
        }


        public bool _Has(string key)
        {
            bool isExist = false;
            string filename = this._GetFileName(key);
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
                T ret = new T();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read))
                {
                    ret = (T)bf.Deserialize(fs);
                }
                return ret;
            }
            else
            {

                //return new T();
                return default(T); //返回null;
                //throw new FileNotFoundException(string.Format("file {0} does not exist", cacheFileName));
            }

        }



    }
}
