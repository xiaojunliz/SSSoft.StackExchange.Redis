using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

//------------------------------------------------------------------------------------//
//  功能描述：Redis缓存操作StackExchange驱动IDatabase扩展
//  创建人：李小军
//  创建时间：2015-11-18
//  修改人：
//  修改时间：
//  支持版本：
//  废弃版本：
//  废弃时间：
//  废弃原因：
//  描述：Redis缓存操作StackExchange驱动IDatabase扩展
//  特殊说明：依赖于Newtonsoft.Json
//------------------------------------------------------------------------------------//
namespace WFSoft.StackExchange.Redis.ExtensionStackExchange
{
    /// <summary>
    /// Redis缓存操作StackExchange驱动IDatabase扩展
    /// </summary>
    public static class ExtensionStackExchangeIDatabase
    {
        #region 缓存操作基于SetString、SetString扩展

        #region 获取缓存数据

        /// <summary>
        /// 根据指定缓存Key获取指定对象数据
        /// </summary>
        /// <typeparam name="T">指定返回对象</typeparam>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定缓存key</param>
        /// <returns>指定对象数据</returns>
        public static T Get<T>(this IDatabase cache, string key)
        {
            string _result = cache.StringGet(key);
            if (!string.IsNullOrWhiteSpace(_result))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(_result);
            return default(T);
        }

        /// <summary>
        /// 根据指定缓存Key获取对象数据(string)
        /// </summary>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定缓存key</param>
        /// <returns>对象数据(string)</returns>
        public static string Get(this IDatabase cache, string key)
        {
            return cache.StringGet(key);
        }

        #endregion

        #region 添加缓存

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定缓存key</param>
        /// <param name="value">缓存值</param>
        /// <param name="expireMinutes">过期时间，不设置代表持久化，单位：分钟</param>
        public static bool Set<T>(this IDatabase cache, string key, T value, int? expireMinutes = null)
        {
            string _setValue = null;
            if (value != null)
                _setValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);

            if (expireMinutes != null && expireMinutes.Value > 0)
            {
                return cache.StringSet(key, _setValue, TimeSpan.FromMinutes(expireMinutes.Value));
            }
            else
            {
                return cache.StringSet(key, _setValue);
            }
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定缓存key</param>
        /// <param name="value">缓存值</param>
        /// <param name="expireMinutes">过期时间，不设置代表持久化，单位：分钟</param>
        public static bool Set(this IDatabase cache, string key, string value, int? expireMinutes = null)
        {
            if (expireMinutes != null && expireMinutes.Value > 0)
            {
                return cache.StringSet(key, value, TimeSpan.FromMinutes(expireMinutes.Value));
            }
            else
            {
                return cache.StringSet(key, value);
            }
        }

        #endregion

        #endregion

        #region 删除指定缓存

        /// <summary>
        /// 删除指定缓存
        /// </summary>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">待删除缓存Key</param>
        /// <returns>是否删除成功</returns>
        public static bool RemoveCache(this IDatabase cache, string key)
        {
            return cache.KeyDelete(key);
        }

        #endregion

        #region 判断指定缓存是否存在

        /// <summary>
        /// 判断指定缓存是否存在
        /// </summary>
        /// <param name="key">待判断缓存Key</param>
        /// <returns>是否存在该缓存</returns>
        public static bool HasKey(this IDatabase cache, string key)
        {
            return cache.KeyExists(key);
        }

        #endregion

        #region 数据操作基于HashSet扩展Get、Set

        #region 获取数据数据(HashSet)

        /// <summary>
        /// 根据指定Key获取指定Hash对应Key对象数据
        /// </summary>
        /// <typeparam name="T">指定返回对象</typeparam>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定数据标识key</param>
        /// <param name="hashKey">获取指定HashKey</param>
        /// <returns>指定对象数据</returns>
        public static T GetHash<T>(this IDatabase cache, string key, string hashKey)
        {
            string _result = cache.HashGet(key, hashKey);
            if (!string.IsNullOrWhiteSpace(_result))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(_result);
            return default(T);
        }

        /// <summary>
        /// 根据指定Key获取指定Hash对应Key对象数据
        /// </summary>
        /// <typeparam name="T">指定返回对象</typeparam>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定缓存key</param>
        /// <param name="hashKey">获取指定HashKey</param>
        /// <returns>指定对象数据</returns>
        public static string GetHash(this IDatabase cache, string key, string hashKey)
        {
            return cache.HashGet(key, hashKey);
        }

        /// <summary>
        /// 根据指定Key获取所有Hash数据
        /// </summary>
        /// <typeparam name="T">指定返回对象</typeparam>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定数据标识Key</param>
        /// <returns>指定Key获取所有Hash数据</returns>
        public static IList<T> GetHashAll<T>(this IDatabase cache, string key)
        {
            RedisValue[] items = cache.HashValues(key);
            if (items != null && items.Length > 0)
            {
                IList<T> values = new List<T>();
                for (int i = 0, j = items.Length; i < j; i++)
                {
                    if (!string.IsNullOrWhiteSpace(items[i]))
                        values.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(items[i]));
                    else
                        values.Add(default(T));
                }
                return values;
            }
            return default(IList<T>);
        }

        #endregion

        #region 添加数据(HashSet)

        /// <summary>
        /// 直接指定Key指定hash Key添加数据
        /// </summary>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定数据标识key</param>
        /// <param name="hashKey">指定Hash key</param>
        /// <param name="value">数据值</param>
        public static bool SetHash<T>(this IDatabase cache, string key, string hashKey, T value)
        {
            string _setValue = null;
            if (value != null)
                _setValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            return cache.HashSet(key, hashKey, _setValue);
        }

        /// <summary>
        /// 直接指定Key指定hash Key添加数据
        /// </summary>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定数据标识key</param>
        /// <param name="hashKey">指定Hash key</param>
        /// <param name="value">数据值</param>
        public static bool SetHash(this IDatabase cache, string key, string hashKey, string value)
        {
            return cache.HashSet(key, hashKey, value);
        }

        #endregion

        #region 删除Hash项数据

        /// <summary>
        /// 删除指定Key指定Hash Key数据
        /// </summary>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定数据标识Key</param>
        /// <param name="hashKey">待删除指定Hash Key</param>
        /// <returns>是否删除成功</returns>
        public static bool HashRemove(this IDatabase cache, string key, string hashKey)
        {
            return cache.HashDelete(key, hashKey);
        }

        #endregion

        #region 获取指定Hash Key项个数

        /// <summary>
        /// 获取指定Hash Key项个数
        /// </summary>
        /// <param name="cache">扩展对象（无需设置）</param>
        /// <param name="key">指定数据标识Key</param>
        /// <returns>指定Hash Key项个数</returns>
        public static long HashItemCount(this IDatabase cache, string key)
        {
            return cache.HashLength(key);
        }

        #endregion

        #endregion
    }
}
