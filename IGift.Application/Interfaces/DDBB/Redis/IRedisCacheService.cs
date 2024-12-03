namespace IGift.Application.Interfaces.DDBB.Redis
{
    public interface IRedisCacheService
    {
        Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> fetchFunc, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
    }
}
