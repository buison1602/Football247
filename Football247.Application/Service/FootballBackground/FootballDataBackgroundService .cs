
using Football247.Domain.Entities;
using Football247.Repositories.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Enum;

namespace Football247.Application.Service.FootballBackground
{
    public class FootballDataBackgroundService : BackgroundService
    {
        private static readonly TimeSpan LiveInterval = TimeSpan.FromMinutes(4);
        private static readonly TimeSpan PreMatchInterval = TimeSpan.FromMinutes(30);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FootballDataBackgroundService> _logger;

        public FootballDataBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<FootballDataBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("FootballDataBackgroundService started.");

            // B1: Sync toàn bộ mùa giải 1 lần khi khởi động
            await RunScopedAsync(job => job.SyncFullSeasonAsync(ct), ct);

            // Vòng lặp daily
            while (!ct.IsCancellationRequested)
            {
                var todayMatches = await GetTodayMatchesFromDbAsync(ct);

                if (todayMatches.Count == 0)
                {
                    // Không có trận hôm nay → ngủ đến 00:05 UTC ngày mai
                    var sleep = GetSleepUntilTomorrow();
                    _logger.LogInformation("No PL matches today. Sleeping {Hours:F1}h until tomorrow.", sleep.TotalHours);
                    await Task.Delay(sleep, ct);
                    continue;
                }

                _logger.LogInformation("Today has {Count} PL matches. Starting watch loop.", todayMatches.Count);

                // B3: Watch loop cho ngày hôm nay
                await WatchTodayAsync(ct);

                // Xong hết trận hôm nay → đợi đến ngày mai
                var sleepAfter = GetSleepUntilTomorrow();
                _logger.LogInformation("All matches done. Sleeping {Hours:F1}h until tomorrow.", sleepAfter.TotalHours);
                await Task.Delay(sleepAfter, ct);
            }
        }

        private async Task WatchTodayAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                SyncTodayResult? result = null;

                await RunScopedAsync(async job =>
                {
                    result = await job.SyncTodayMatchesAsync(ct);

                    // B4: Có match vừa FINISHED → sync standings ngay
                    if (result.AnyJustFinished)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), ct); // đợi 5s cho API cập nhật
                        await job.SyncStandingsAsync(ct);
                    }
                }, ct);

                // Tất cả trận kết thúc → thoát watch loop
                if (result?.AllFinished == true)
                {
                    _logger.LogInformation("All today's matches finished. Exiting watch loop.");
                    break;
                }

                // Chọn interval tiếp theo
                var interval = result?.HasLiveMatch == true ? LiveInterval : PreMatchInterval;
                _logger.LogInformation("Next sync in {Minutes} min (HasLive={HasLive})",
                    interval.TotalMinutes, result?.HasLiveMatch);

                await Task.Delay(interval, ct);
            }
        }

        private async Task<List<Match>> GetTodayMatchesFromDbAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return uow.MatchRepository.ReadQueryable
                .Where(m =>
                    m.CompetitionCode == "PL"
                    && DateOnly.FromDateTime(m.UtcDate) == today
                    && m.Status != EnumMatchStatus.Postponed
                    && m.Status != EnumMatchStatus.Cancelled)
                .ToList();
        }

        // Helper tạo scope và chạy job
        private async Task RunScopedAsync(Func<FootballSyncJob, Task> action, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService<FootballSyncJob>();
            try
            {
                await action(job);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in sync job.");
            }
        }

        private static TimeSpan GetSleepUntilTomorrow()
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.Date.AddDays(1).AddMinutes(5); // 00:05 UTC ngày mai
            return tomorrow - now;
        }
    }
}
