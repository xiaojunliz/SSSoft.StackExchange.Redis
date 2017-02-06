using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//------------------------------------------------------------------------------------//
//  功能描述：Redis缓存操作StackExchange驱动IServer扩展
//  创建人：李小军
//  创建时间：2015-11-18
//  修改人：
//  修改时间：
//  支持版本：
//  废弃版本：
//  废弃时间：
//  废弃原因：
//  描述：Redis缓存操作StackExchange驱动IServer扩展
//  特殊说明：依赖于Newtonsoft.Json
//------------------------------------------------------------------------------------//
namespace WFSoft.StackExchange.Redis.ExtensionStackExchange
{
    /// <summary>
    /// Redis缓存操作StackExchange驱动IServer扩展
    /// </summary>
    public static class ExtensionStackExchangeIServer
    {
        /// <summary>
        /// 根据指定Redis数据库索引以及筛选条件获取数据库所有Key
        /// </summary>
        /// <typeparam name="T">指定返回对象</typeparam>
        /// <param name="iserver">扩展对象（无需设置）</param>
        /// <param name="searchKey">筛选关键字</param>
        /// <param name="database">指定数据库索引</param>
        /// <returns>指定Redis数据库索引以及筛选条件获取数据库所有Key</returns>
        public static IList<string> GetAllKeys(this IServer iserver,string searchKey, int database)
        {
            return iserver.Keys(database, searchKey).Select(s => s.ToString()).ToList();
        }

        /// <summary>
        /// 根据指定Redis数据库索引以及筛选条件获取数据库所有数据
        /// </summary>
        /// <typeparam name="T">指定返回对象</typeparam>
        /// <param name="iserver">扩展对象（无需设置）</param>
        /// <param name="searchKey">筛选关键字</param>
        /// <param name="db">实例化后的数据库操作对象</param>
        /// <param name="database">指定数据库索引</param>
        /// <returns>指定Redis数据库索引以及筛选条件获取数据库所有数据</returns>
        public static IList<string> GetAll(this IServer iserver, IDatabase db,string searchKey, int database)
        {
            IList<string> allKeys = iserver.Keys(database, searchKey).Select(s => s.ToString()).ToList();
            if (allKeys != null && allKeys.Count > 0)
            {
                IList<string> allValues = new List<string>();
                foreach (string key in allKeys)
                {
                    allValues.Add(db.Get(key));
                }
                return allValues;
            }
            return default(IList<string>);
        }

        /// <summary>
        /// 根据指定Redis数据库索引以及筛选条件获取数据库所有数据
        /// </summary>
        /// <typeparam name="T">指定返回对象</typeparam>
        /// <param name="iserver">扩展对象（无需设置）</param>
        /// <param name="searchKey">筛选关键字</param>
        /// <param name="db">实例化后的数据库操作对象</param>
        /// <param name="database">指定数据库索引</param>
        /// <returns>指定Redis数据库索引以及筛选条件获取数据库所有数据</returns>
        public static IList<T> GetAll<T>(this IServer iserver, IDatabase db, string searchKey, int database)
        {
            IList<string> allKeys = iserver.Keys(database, searchKey).Select(s => s.ToString()).ToList();
            if (allKeys != null && allKeys.Count > 0)
            {
                IList<T> allValues = new List<T>();
                foreach (string key in allKeys)
                {
                    string tempValue = db.Get(key);
                    if (!string.IsNullOrWhiteSpace(tempValue))
                    {
                        allValues.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(tempValue));
                    }
                }
                return allValues;
            }
            return default(IList<T>);
        }
    }
}
