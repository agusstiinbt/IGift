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

        /// <summary>
        /// Obtiene un valor del caché o lo establece si no existe, utilizando un delegado para obtener los datos.
        /// </summary>
        public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> fetchFunc, TimeSpan? expiry = null)
        {
            var db = _redis.GetDatabase();
            var cachedValue = await db.StringGetAsync(key);

            if (cachedValue.HasValue)
            {
                return _jsonSerializer.Deserialize<T>(cachedValue);
            }

            // Si no hay datos en caché, ejecuta el delegado para obtener los datos
            var data = await fetchFunc();

            if (data != null)
            {
                await SetAsync(key, data, expiry);
            }

            return data;
        }

        /// <summary>
        /// Establece un valor en el caché.
        /// </summary>
        private async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var db = _redis.GetDatabase();
            var serialized = _jsonSerializer.Serialize(value);
            await db.StringSetAsync(key, serialized, expiry);
        }

        /// <summary>
        /// Elimina un valor del caché.
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }
}
