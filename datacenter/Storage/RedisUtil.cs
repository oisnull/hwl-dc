using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models.Storage
{
    public class RedisUtil
    {
        public const string HOST = "127.0.0.1";
        public const int PORT = 6379;
        public const string AUTH = "123456";

        //public const int MAX_ACTIVE = 1024;
        //public const int MAX_IDLE = 50;
        //public const int MAX_WAIT = 10000;
        //public const int TIMEOUT = 10000;

        private static object _locker = new Object();
        private static ConnectionMultiplexer _instance = null;

        public static ConnectionMultiplexer Client
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = ConnectionMultiplexer.Connect(string.Format("{0}:{1},allowadmin=true,password={2}", HOST, PORT, AUTH));
                        }
                    }
                }
                return _instance;
            }
        }

        public static T Exec<T>(Func<IDatabase, T> func, int dbNum = 0)
        {
            var database = Client.GetDatabase(dbNum);
            return func(database);
        }

        public static void Exec(Action<IDatabase> func, int dbNum = 0)
        {
            var database = Client.GetDatabase(dbNum);
            func(database);
        }
    }
}
