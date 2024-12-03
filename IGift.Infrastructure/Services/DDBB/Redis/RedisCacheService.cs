using IGift.Application.Interfaces.DDBB.Redis;
using IGift.Application.Interfaces.Serialization.Options;
using StackExchange.Redis;

namespace IGift.Infrastructure.Services.DDBB.Redis
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IJsonSerializer _jsonSerializer;

        public RedisCacheService(IConnectionMultiplexer redis, IJsonSerializer jsonSerializer)
        {
            _redis = redis;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            return value.HasValue ? _jsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var db = _redis.GetDatabase();
            var serialized = _jsonSerializer.Serialize(value);
            await db.StringSetAsync(key, serialized, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }
}
