using Football247.Domain.Models.EntityModels.DTOs.Match;
using Football247.Domain.Models.EntityModels.DTOs.Standing;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Football247.Application.Service.FootballBackground
{
    public class FootballDataClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<FootballDataClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FootballDataClient(HttpClient http, ILogger<FootballDataClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        // B1: Lấy toàn bộ mùa giải 1 lần khi start
        public async Task<string> GetFullSeasonAsync(string competition, CancellationToken ct = default)
        {
            return await GetAsync($"/v4/competitions/{competition}/matches", ct);
        }

        // B3: Lấy trận của 1 ngày cụ thể (dùng để watch hôm nay)
        public async Task<string> GetMatchesByDateAsync(string competition, DateOnly date, CancellationToken ct = default)
        {
            var d = date.ToString("yyyy-MM-dd");
            return await GetAsync($"/v4/competitions/{competition}/matches?dateFrom={d}&dateTo={d}", ct);
        }

        // B4: Lấy bảng xếp hạng
        public async Task<string> GetStandingsAsync(string competition, CancellationToken ct = default)
        {
            return await GetAsync($"/v4/competitions/{competition}/standings", ct);
        }

        private async Task<string> GetAsync(string url, CancellationToken ct)
        {
            _logger.LogDebug("GET {Url}", url);
            var response = await _http.GetAsync(url, ct);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromMinutes(1);
                _logger.LogWarning("Rate limit hit. Retry after {Seconds}s", retryAfter.TotalSeconds);
                throw new RateLimitException(retryAfter);
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
