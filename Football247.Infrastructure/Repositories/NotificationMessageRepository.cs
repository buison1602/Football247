using Football247.Domain.Entities;
using Football247.Domain.IRepositories;
using Football247.Repositories;

namespace Football247.Infrastructure.Repositories
{
    public class NotificationMessageRepository : Repository<NotificationMessage>, INotificationMessageRepository
    {
        private readonly Football247DbContext _db;

        public NotificationMessageRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }
    }
}
