using Football247.Application.Service.UserService;
using Football247.Domain.Entities;
using Football247.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Football247.Infrastructure.Persistence
{
    public abstract class BaseDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        protected BaseDbContext(DbContextOptions options, ICurrentUserService currentUserService, IMediator mediator) : base(options)
        {
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
   
            var entityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType) && !t.ClrType.IsAbstract);
        
            var method = typeof(BaseDbContext).GetMethod(nameof(ApplyGlobalQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var entityType in entityTypes)
            {
                method?.MakeGenericMethod(entityType.ClrType).Invoke(this, new object[] { modelBuilder });
            }
        }

        private void ApplyGlobalQueryFilter<T>(ModelBuilder builder) where T : BaseEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchDomainEventsAsync();
            return result;
        }

        private void BeforeSaveChanges()
        {
            var userId = _currentUserService.UserId;    
            var fullName = _currentUserService.FullName;    
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Id = entry.Entity.Id == Guid.Empty ? Guid.NewGuid() : entry.Entity.Id;
                        entry.Entity.CreatedDate = now;
                        entry.Entity.CreatedUserId = userId;
                        entry.Entity.CreatedFullName = fullName;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = now;
                        entry.Entity.UpdatedUserId = userId;
                        entry.Entity.UpdatedFullName = fullName;
                        break;

                    case EntityState.Deleted:
                        // Chặn lệnh Xóa cứng, biến nó thành Cập nhật (Xóa mềm)
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedDate = now;
                        entry.Entity.DeletedUserId = userId;
                        entry.Entity.DeletedFullName = fullName;
                        break;
                }
            }
        }


        private async Task DispatchDomainEventsAsync()
        {
            // Tương lai bạn sẽ dùng hàm này để xử lý Domain Events (VD: Khi Article được tạo -> Gửi Email)
            await Task.CompletedTask;
        }
    }
}
