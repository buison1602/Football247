using Microsoft.Extensions.Logging;
using System.Net;

namespace Football247.Application.Service.FootballBackground
{
    public class FootballDataClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<FootballDataClient> _logger;

        public FootballDataClient(HttpClient http, ILogger<FootballDataClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        // Lấy toàn bộ danh sách trận của 1 competition
        public Task<string> GetAllMatchesAsync(string competition, CancellationToken ct = default)
            => GetAsync($"competitions/{competition}/matches", ct);

        // Lấy bảng xếp hạng
        public Task<string> GetStandingsAsync(string competition, CancellationToken ct = default)
            => GetAsync($"competitions/{competition}/standings", ct);

        private async Task<string> GetAsync(string path, CancellationToken ct)
        {
            _logger.LogDebug("[FootballDataClient] GET {Path}", path);

            var response = await _http.GetAsync(path, ct);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var retry = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
                _logger.LogWarning("[FootballDataClient] Rate limit! Retry after {Sec}s", retry.TotalSeconds);
                throw new RateLimitException(retry);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                _logger.LogError("[FootballDataClient] 403 Forbidden — kiểm tra ApiToken!");
                throw new UnauthorizedAccessException("API Token không hợp lệ.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }
    }

    public class RateLimitException(TimeSpan retryAfter) : Exception("Rate limit exceeded")
    {
        public TimeSpan RetryAfter { get; } = retryAfter;
    }
}