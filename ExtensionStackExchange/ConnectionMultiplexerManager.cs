using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//------------------------------------------------------------------------------------//
//  功能描述：Redis连接对象代理类，基于StackExchange驱动
//  创建人：李小军
//  创建时间：2015-11-18
//  修改人：
//  修改时间：
//  支持版本：
//  废弃版本：
//  废弃时间：
//  废弃原因：
//  描述：基于StackExchange驱动
//------------------------------------------------------------------------------------//
namespace WFSoft.StackExchange.Redis.ExtensionStackExchange
{
    /// <summary>
    /// Redis连接对象代理类，基于StackExchange驱动
    /// </summary>
    public static class ConnectionMultiplexerManager
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object _locker = new object();

        /// <summary>
        /// ConnectionMultiplexer对象是StackExchange.Redis最中枢的对象，这个类的实例需要被整个应用程序域共享和重用的，
        /// 你不要在每个操作中不停的创建该对象的实例，所以使用单例来创建和存放这个对象是必须的
        /// </summary>
        private static ConnectionMultiplexer _redis;

        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public static string _redisConnectionString
        {
            get {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CacheRedisConnection"].ConnectionString;
                if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception("Redis连接字符串未配置！");
                return connectionString;
            }
        }

        /// <summary>
        /// Redis StackExchange.Redis连接访问对象(单例)
        /// </summary>
        public static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis != null) return _redis;

                        _redis = GetManager();
                        return _redis;
                    }
                }
                return _redis;
            }
        }

        /// <summary>
        /// 实例化Redis StackExchange.Redis连接访问对象
        /// </summary>
        /// <param name="connectionString">Redis连接字符串</param>
        /// <returns>Redis StackExchange.Redis连接访问对象</returns>
        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _redisConnectionString;
            }

            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
            options.AllowAdmin = true;
            return ConnectionMultiplexer.Connect(options);
        }
    }
}
