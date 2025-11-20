namespace Football247.Services.Caching
{
    public interface IRedisCacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task SetDataAsync<T>(string key, T data);
        Task RemoveDataAsync(string key);
    }
}
