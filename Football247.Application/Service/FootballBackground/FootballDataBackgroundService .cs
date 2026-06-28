using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Football247.Application.Service.FootballBackground
{
    public class FootballDataBackgroundService : BackgroundService
    {
        // Cứ 1 phút gọi 1 lần → 2 competitions = 2 requests/phút, an toàn
        private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<FootballDataBackgroundService> _logger;

        public FootballDataBackgroundService(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<FootballDataBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("\n\n\nFootballDataBackgroundService started+++++++++++++++++++++++++++++++++++++++++++++++++++++++++.\n\n\n\n");

            // Đọc danh sách competitions từ config: ["WC", "PL"]
            var competitions = _config
                .GetSection("FootballData:Competitions")
                .Get<string[]>() ?? new[] { "WC" };

            // Chờ app khởi động xong rồi mới bắt đầu
            await Task.Delay(TimeSpan.FromSeconds(5), ct);

            while (!ct.IsCancellationRequested)
            {
                _logger.LogInformation("[FootballBG] Bắt đầu poll cycle...");

                foreach (var competition in competitions)
                {
                    if (ct.IsCancellationRequested) break;

                    await RunAsync(competition, ct);

                    // Delay 3 giây giữa mỗi competition
                    // → tránh gửi 2 request cùng lúc, an toàn với rate limit
                    if (competition != competitions.Last())
                        await Task.Delay(TimeSpan.FromSeconds(3), ct);
                }

                _logger.LogInformation(
                    "[FootballBG] Poll cycle xong. Chờ {Min} phút...",
                    PollInterval.TotalMinutes);

                await Task.Delay(PollInterval, ct);
            }

            _logger.LogInformation("FootballDataBackgroundService stopped.");
        }

        private async Task RunAsync(string competition, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService<FootballSyncJob>();

            try
            {
                // Gọi song song 2 API: matches + standings
                // Mỗi cái 1 request → tổng 2 requests / competition
                await Task.WhenAll(
                    job.SyncMatchesAsync(competition, ct),
                    job.SyncStandingsAsync(competition, ct)
                );

                await job.SyncLiveMatchDetailsAsync(ct);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FootballBG] Lỗi khi sync {C}", competition);
            }
        }
    }
}