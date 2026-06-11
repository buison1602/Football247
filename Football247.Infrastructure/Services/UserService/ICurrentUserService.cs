namespace Football247.Application.Service.UserService
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string FullName { get; }
    }
}
