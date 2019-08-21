本地缓存策略是存在了内存字典集合里，而Redis缓存把数据存放到了数据库里。Redis相关的库基本上会提供类似Manager之类用来连接数据库的类。首先来一个帮助类可以根据连接字符串生成Manager。

```
public class RedisManager
{
    //对外开放一个属性，用来存放连接字符串
    public static string ConfigurationOption{get;set;}

    //根据连接字符串生成Redis特有的帮助类ConnectionMultiplexer
    //也就是可以通过两种方式获取链接字符串，一种是通过这里的方法，一种是通过属性
    private static ConnectionMuliplexer GetManager(string connectionString=null)
    {
        if(string.IsNullOrEmpty(connectionString))
        {
            if(ConfigurationOption == null)
            {
                connectionString=GetDefaultConnectionString();
            }
            else
            {
                return ConnectionMultiplexer.Connection(ConfigurationOptions.Parse(ConfigurationOption));
            }
        }
        return ConnectionMultplexer.Connect(connectionString);
    }

    private static string GetDefaultConnectionString()
    {
        return "localhost";
    }

    //ConnectionMultiplexer通过单例的方式开放出去
    internal static ConnectionMultiplexer _redis
    {
        get
        {
            return Nested.instance;
        }
    }

    class Nested
    {
        static Nested(){}
        internal static readonly ConnectionMultiplexer instance = GetManager();
    }

    public static ConnectionMultiplexer Manager
    {
        get
        {
            return _redis;
        }
    }
}
```

以上，可以单例获取到了`ConnectionMultiplexer`，接下了就是实现缓存接口。

```
public class RedisObjectCacheStrategy : BaseCacheStrategy, IObjectCacheStrategy
{
    internal ConnectionMultiplexer _client;

    public RedisObjectCacheStrategy()
    {
        _client = RedisManager.Manager;
    }

    ~RedisObjectCacheStrategy()
    {
        _client.Dispose();
    }

    //其它缓存策略的常规操作和常规方法
}

```