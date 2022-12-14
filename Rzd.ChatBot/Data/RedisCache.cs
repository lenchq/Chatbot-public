using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rzd.ChatBot.Data.Interfaces;
using Rzd.ChatBot.Types.Options;
using StackExchange.Redis;

namespace Rzd.ChatBot.Data;

public class RedisCache<TData> : IMemoryCache<TData>
{
    private readonly IDatabase _db;
    private readonly ConnectionMultiplexer _connection;
    private readonly RedisOptions _options;
    private readonly ILogger<RedisCache<TData>> _logger;

    public RedisCache(ILogger<RedisCache<TData>> logger,IOptions<RedisOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        try
        {
            _connection = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = new() { _options.Url },
                //User = _options.User,
                //Password = _options.Password,
                ReconnectRetryPolicy = new LinearRetry(60_000),
            });
        }
        catch (Exception ex)
        {
            throw new RedisConnectionException(ConnectionFailureType.UnableToResolvePhysicalConnection,
                "Cannot connect to Redis server", ex);
        }
        _db = _connection.GetDatabase();
    }

    public TData this[string key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    public TData Get(string key)
    {
        var json = _db.StringGet(key);
        if (json.IsNullOrEmpty)
            throw new ArgumentException("No such key exists", nameof(key));
        var obj = JsonParse<TData>(json!);
        return obj;
    }

    public void Set(string key, TData value)
    {
        var stringified = JsonStringify(value);
        var result = _db.StringSet(key, stringified);
        if (!result)
        {
            throw new Exception($"Something went wrong when setting {key} to {stringified}");
        }
    }

    public void Delete(string key)
    {
        _db.KeyDelete(key);
    }

    public void Set(string key, TData value, TimeSpan expiry)
    {
        var stringified = JsonStringify(value);
        var result = _db.StringSet(key, stringified, expiry);
        if (!result)
        {
            throw new Exception($"Something went wrong when setting {key} to {stringified}");
        }
    }

    private static string JsonStringify(TData value)
        => JsonConvert.SerializeObject(value);
    private static T JsonParse<T>(string jsonText)
        => JsonConvert.DeserializeObject<T>(jsonText);

    public void Dispose()
    {
        // _connection.GetServers()
        //     .ToList()
        //     .ForEach(x => x.Shutdown());
    }
}