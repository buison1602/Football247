namespace Football247.Services.IService
{
    public interface IRealtimeService
    {
        Task NotifyUserAsync(string userId, string method, object data);
        Task NotifyAllAsync(string method, object data);
    }
}
