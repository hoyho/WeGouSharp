using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WeGouSharp
{
    class CacheHelper
    {

        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="CacheKey">键</param>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string CacheKey, object objObject, TimeSpan Timeout)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, DateTime.MaxValue, Timeout, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public static void RemoveAllCache(string CacheKey)
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            _cache.Remove(CacheKey);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }
        }
    }


    public static class FileBasedCahce
    {
        static Dictionary<string, string> _FileMap;
        const string MAPFILENAME = "FileBasedCahceMAP.dat";
        public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static FileBasedCahce()
        {
            if (!Directory.Exists(DirectoryLocation))
                throw new ArgumentException("directoryLocation msu exist");
            if (File.Exists(MyMapFileName))
            {
                _FileMap = DeSerializeFromBin<Dictionary<string, string>>(MyMapFileName);
            }
            else
                _FileMap = new Dictionary<string, string>();
        }
        public static T Get<T>(string key) where T : new()
        {
            if (_FileMap.ContainsKey(key))
                return (T)DeSerializeFromBin<T>(_FileMap[key]);
            else
                throw new ArgumentException("Key not found");
        }
        public static void Insert<T>(string key, T value)
        {
            if (_FileMap.ContainsKey(key))
            {
                SerializeToBin(value, _FileMap[key]);
            }
            else
            {
                _FileMap.Add(key, GetNewFileName);
                SerializeToBin(value, _FileMap[key]);
            }
            SerializeToBin(_FileMap, MyMapFileName);
        }
        private static string GetNewFileName
        {
            get
            {
                return Path.Combine(DirectoryLocation, Guid.NewGuid().ToString());
            }
        }
        private static string MyMapFileName
        {
            get
            {
                return Path.Combine(DirectoryLocation, MAPFILENAME);
            }
        }
        private static void SerializeToBin(object obj, string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                bf.Serialize(fs, obj);
            }
        }
        private static T DeSerializeFromBin<T>(string filename) where T : new()
        {
            if (File.Exists(filename))
            {
                T ret = new T();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    ret = (T)bf.Deserialize(fs);
                }
                return ret;
            }
            else
                throw new FileNotFoundException(string.Format("file {0} does not exist", filename));
        }

    }

}
