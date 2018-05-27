using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WeGouSharp
{


    class WechatCache
    {
        private readonly FileSystemCache _fileCache;

        public WechatCache(string cacheDir, int defaultTimeOut)
        {
            //cache_dir是缓存目录
            _fileCache = new FileSystemCache(cacheDir, 500, defaultTimeOut);
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        /// <returns></returns>
        public bool ClearAll()
        {
            return _fileCache._Clear();
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
            return _fileCache._Get<T>(key);
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
            return _fileCache._Add(key, value, timeOut);
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
            return _fileCache._Update(key, value, timeOut);
        }


        /// <summary>
        /// 添加一个文件缓存,如果键值key对应的缓存不存在，那么增加值value缓存，，否则返回false；
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Has(string key)
        {
            return _fileCache._Has(key);
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
            return _fileCache._Delete(key);
        }
    }


 
}
