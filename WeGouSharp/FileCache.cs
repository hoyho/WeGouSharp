using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WeGouSharp
{
    class FileCache
    {
    }

    class WechatCache
    {
        FileSystemCache _cache;
        public WechatCache(string CacheDir, int DefaultTimeOut)
        {
            //cache_dir是缓存目录
            this._cache = new FileSystemCache(CacheDir, 500, DefaultTimeOut);
        }
        public bool clear()
        {
            return this._cache.Clear();
        }

        public string get(string key)
        {
            //获取键值key的缓存值
          //如果没有对应缓存，返回None
            return this._cache.get(key);
        }

        public bool add(string key,string value,int timeOut)
        {
            //增加缓存
            //如果键值key对应的缓存不存在，那么增加值value到键值key，过期时间timeout，默认300秒
            //否则返回False（即不能覆盖设置缓存）
            return this._cache.add(key, value, timeOut);
        }

        public bool set(string key,string value,int timeOut)
        {
            //设置缓存
            //设置键值key的缓存为value,过期时间300秒
            return this._cache.set(key, value, timeOut);
        }
        public bool delete(string key)
        {
            //删除缓存
        //删除键值key存储的缓存
            return this._cache.delete(key);
        }
}


    class BaseCache
    {
        int _defaultTimeout;
        public BaseCache(int DefaultTimeOut)
        {
            _defaultTimeout = DefaultTimeOut;
        }


        //public int NormalizeTimeout(int timeout)
        //{
        //    if (timeout)
        //    {
        //        timeout = _defaultTimeout;
        //    }
        //    return timeout;
        //}

        public string get(string  key)
        {
            //Look up key in the cache and return the value for it.

            //:param key: the key to be looked up.
            //returns: The value if it exists and is readable, else ``None``.
            //"""
            return null;
        }


        public bool delete(string key)
        {
            //    """Delete `key` from the cache.
            //:param key: the key to delete.
            //:returns: Whether the key existed and has been deleted.
            //:rtype: boolean

            return true;

        }
  
        

    }


    class FileSystemCache
    {
        string _path;
        int _threshold;

        //used for temporary files by the FileSystemCache
        string _fs_transaction_suffix = ".__wz_cache";

        public FileSystemCache(string cacheDirectory,int threshold=500,int defaultTimeout=300)
        {
            //BaseCache.__init__(self, default_timeout)
            this._path = cacheDirectory;
            this._threshold = threshold;
            try
            {
                if(!Directory.Exists(cacheDirectory))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }
            }catch(Exception e)
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
        public List<string >  ListCache()

        {        //return a list of (fully qualified) cache filenames

            //return [os.path.join(self._path, fn) for fn in os.listdir(self._path)
            //    if not fn.endswith(self._fs_transaction_suffix)]

            List<string> cacheList = new List<string> { };
            foreach(var fileName in Directory.EnumerateFiles(this._path))
            {
                if (!fileName.EndsWith(this._fs_transaction_suffix))
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


        public bool Clear()
        {
            foreach(string fname in this.ListCache())
            {
                try
                {
                    File.Delete(fname);
                } catch(Exception e)
                {
                    return false;
                }
               
            }

            return true;
        }

        private string  _get_filename(string key)
        {
            byte[] bytes = Encoding.Default.GetBytes(key);
            key = Encoding.UTF8.GetString(bytes);
   


            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(key);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString());

            }

            return sb.ToString();
            //    key = key.encode('utf-8')  # XXX unicode review
            //hash = md5(key).hexdigest()

            //return os.path.join(self._path, hash)
        }

        public string  get(string key)
        {
            string filename = this._get_filename(key);
            return "";
        //    filename = self._get_filename(key)
        //try:
        //    with open(filename, 'rb') as f:
        //        pickle_time = pickle.load(f)
        //        if pickle_time == 0 or pickle_time >= time():
        //            return pickle.load(f)
        //        else:
        //            os.remove(filename)
        //            return None
        //except(IOError, OSError, pickle.PickleError):
        //    return None

        }

        public bool add(string key, string value, int timeout  )
        {
            string filename = this._get_filename(key);
           if(!Directory.Exists(filename))
            {
                return this.set(key, value, timeout);
            }else
            {
                return false;
            }


        }


    public bool set(string key,string  value,int timeout)
        {
            timeout = this._NormalizeTimeout(timeout);
            string filename = this._get_filename(key);
            this._prune();

            //    try:
            //    fd, tmp = tempfile.mkstemp(suffix = self._fs_transaction_suffix,
            //                               dir = self._path)
            //    with os.fdopen(fd, 'wb') as f:
            //        pickle.dump(timeout, f, 1)
            //        pickle.dump(value, f, pickle.HIGHEST_PROTOCOL)
            //    rename(tmp, filename)
            //    os.chmod(filename, self._mode)
            //except(IOError, OSError):
            //    return False
            //else:
            //    return True
            return true;
        }



    public  bool delete(string key)
        {

            return false;
        //    try:
        //    os.remove(self._get_filename(key))
        //except(IOError, OSError):
        //    return False
        //else:
        //    return True
        }


        public bool has(string key)
        {
            //    filename = self._get_filename(key)
            //try:
            //    with open(filename, 'rb') as f:
            //        pickle_time = pickle.load(f)
            //        if pickle_time == 0 or pickle_time >= time():
            //            return True
            //        else:
            //            os.remove(filename)
            //            return False
            //except(IOError, OSError, pickle.PickleError):
            //    return False
            return false ;
        }

      



    }




}
