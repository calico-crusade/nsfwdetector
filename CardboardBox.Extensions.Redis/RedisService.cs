using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CardboardBox.Extensions
{
    public interface IRedisService
    {
        Task<T> Get<T>(string key, T def = default);
        Task<T[]> Get<T>(params string[] keys);
        Task<Tuple<T0, T1, T2, T3>> Get<T0, T1, T2, T3>(string key1, string key2, string key3, string key4);
        Task<Tuple<T0, T1, T2>> Get<T0, T1, T2>(string key1, string key2, string key3);
        Task<Tuple<T0, T1>> Get<T0, T1>(string key1, string key2);
        Task<bool> Set<T>(string key, T data);
        Task Subscribe(string channel, Action<RedisChannel, RedisValue> action);
        Task Subscribe<T>(string channel, Action<RedisChannel, T> action);
        Task<bool> Delete(string key);
        Task Publish(string channel, RedisValue message);
        Task Publish<T>(string channel, T result);
    }

    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _multiplexer;

        private IDatabase Database => _multiplexer.GetDatabase();
        private ISubscriber Subscriber => _multiplexer.GetSubscriber();

        public RedisService(IConnectionMultiplexer multiplexer)
        {
            _multiplexer = multiplexer ?? throw new ArgumentNullException("multiplexer");
        }

        public async Task<T> Get<T>(string key, T def = default)
        {
            var value = await Database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return def;

            return JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<bool> Set<T>(string key, T data)
        {
            var str = JsonConvert.SerializeObject(data);
            return await Database.StringSetAsync(key, str);
        }

        public async Task Subscribe(string channel, Action<RedisChannel, RedisValue> action)
        {
            await Subscriber.SubscribeAsync(channel, action);
        }

        public async Task Subscribe<T>(string channel, Action<RedisChannel, T> action)
        {
            await Subscribe(channel, (c, v) => action(c, JsonConvert.DeserializeObject<T>(v)));
        }

        public async Task<bool> Delete(string key)
        {
            return await Database.KeyDeleteAsync(key);
        }

        public async Task Publish(string channel, RedisValue message)
        {
            await Subscriber.PublishAsync(channel, message);
        }

        public async Task Publish<T>(string channel, T result)
        {
            await Subscriber.PublishAsync(channel, JsonConvert.SerializeObject(result));
        }

        public async Task<T[]> Get<T>(params string[] keys)
        {
            var values = await Database.StringGetAsync(keys.Select(t => (RedisKey)t).ToArray());

            return values.Select(t => t.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<T>(t)).ToArray();
        }

        public async Task<Tuple<T0, T1>> Get<T0, T1>(string key1, string key2)
        {
            var values = await Database.StringGetAsync(new[] { (RedisKey)key1, (RedisKey)key2 });

            T0 v0 = default;
            T1 v1 = default;

            if (values.Length > 0 && !values[0].IsNullOrEmpty)
                v0 = JsonConvert.DeserializeObject<T0>(values[0]);

            if (values.Length > 1 && !values[1].IsNullOrEmpty)
                v1 = JsonConvert.DeserializeObject<T1>(values[1]);

            return new Tuple<T0, T1>(v0, v1);
        }

        public async Task<Tuple<T0, T1, T2>> Get<T0, T1, T2>(string key1, string key2, string key3)
        {
            var values = await Database.StringGetAsync(new[] { (RedisKey)key1, (RedisKey)key2, (RedisKey)key3 });

            T0 v0 = default;
            T1 v1 = default;
            T2 v2 = default;

            if (values.Length > 0 && !values[0].IsNullOrEmpty)
                v0 = JsonConvert.DeserializeObject<T0>(values[0]);

            if (values.Length > 1 && !values[1].IsNullOrEmpty)
                v1 = JsonConvert.DeserializeObject<T1>(values[1]);


            if (values.Length > 2 && !values[2].IsNullOrEmpty)
                v2 = JsonConvert.DeserializeObject<T2>(values[2]);

            return new Tuple<T0, T1, T2>(v0, v1, v2);
        }

        public async Task<Tuple<T0, T1, T2, T3>> Get<T0, T1, T2, T3>(string key1, string key2, string key3, string key4)
        {
            var values = await Database.StringGetAsync(new[] { (RedisKey)key1, (RedisKey)key2, (RedisKey)key3, (RedisKey)key4 });

            T0 v0 = default;
            T1 v1 = default;
            T2 v2 = default;
            T3 v3 = default;

            if (values.Length > 0 && !values[0].IsNullOrEmpty)
                v0 = JsonConvert.DeserializeObject<T0>(values[0]);

            if (values.Length > 1 && !values[1].IsNullOrEmpty)
                v1 = JsonConvert.DeserializeObject<T1>(values[1]);


            if (values.Length > 2 && !values[2].IsNullOrEmpty)
                v2 = JsonConvert.DeserializeObject<T2>(values[2]);

            if (values.Length > 3 && !values[3].IsNullOrEmpty)
                v3 = JsonConvert.DeserializeObject<T3>(values[3]);

            return new Tuple<T0, T1, T2, T3>(v0, v1, v2, v3);
        }
    }
}
