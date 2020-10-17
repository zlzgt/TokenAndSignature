using Microsoft.Extensions.Caching.Memory;
using System;

namespace TokenAndSignature.Tools
{
    public class CacheHelper
    {
        public static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 缓存绝对过期时间
        /// </summary>
        ///<param name="key">Cache键值</param>
        ///<param name="value">给Cache[key]赋的值</param>
        ///<param name="minute">minute分钟后绝对过期</param>
        public static void CacheInsertAddMinutes(string key, object value, int minute)
        {
            if (value == null) return;
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(minute)));
        }

        /// <summary>
        /// 缓存相对过期，最后一次访问后minute分钟后过期
        /// </summary>
        ///<param name="key">Cache键值</param>
        ///<param name="value">给Cache[key]赋的值</param>
        ///<param name="minute">滑动过期分钟</param>
        public static void CacheInsertFromMinutes(string key, object value, int minute)
        {
            if (value == null) return;
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(minute)));
        }

        /// <summary>
        ///以key键值，把value赋给Cache[key].如果不主动清空，会一直保存在内存中.
        /// </summary>
        ///<param name="key">Cache键值</param>
        ///<param name="value">给Cache[key]赋的值</param>
        public static void CacheInsert(string key, object value)
        {
            _memoryCache.Set(key, value);
        }

        /// <summary>
        ///清除Cache[key]的值
        /// </summary>
        ///<param name="key"></param>
        public static void CacheNull(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <summary>
        ///根据key值，返回Cache[key]的值
        /// </summary>
        ///<param name="key"></param>
        public static object CacheValue(string key)
        {
            return _memoryCache.Get(key);
        }
    }
}
