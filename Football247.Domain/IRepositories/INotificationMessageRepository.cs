using Football247.Domain.Entities;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.IRepositories
{
    public interface INotificationMessageRepository : IRepository<NotificationMessage>
    {
    }
}
