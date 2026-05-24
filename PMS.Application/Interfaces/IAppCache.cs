namespace PMS.Application.Interfaces
{
    public interface IAppCache
    {
        Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory);
        void Remove(string key);
    }
}