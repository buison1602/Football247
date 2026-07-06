using Football247.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Football247.Application.Common.Data
{
    public static class Extensions
    {
        public static async Task ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<Football247DbContext>();
            var logger = services.GetRequiredService<ILogger<WebApplication>>();

            int maxRetries = 6;
            int delaySeconds = 5;

            logger.LogInformation("⏳ [DEVOPS]: Bắt đầu kiểm tra và đồng bộ trạng thái Database...");

            for (int i = 1; i <= maxRetries; i++)
            {
                try
                {
                    // Thử thách kết nối để đảm bảo SQL Server đã qua giai đoạn RECOVERING
                    await dbContext.Database.CanConnectAsync();

                    // Thực hiện nạp cấu trúc các bảng Migration
                    await dbContext.Database.MigrateAsync();

                    logger.LogInformation("✅ [DEVOPS]: Khởi tạo cấu trúc các bảng và Migration thành công!");
                    return;
                }
                catch (Exception ex)
                {
                    // Bẫy lỗi phòng thủ: Nếu dính đúng lỗi "already exists" do xung đột tốc độ (Race Condition)
                    if (ex.Message.Contains("already exists") || (ex.InnerException?.Message?.Contains("already exists") ?? false))
                    {
                        logger.LogWarning("⚠️ [DEVOPS]: Phát hiện DB đã tồn tại trên ổ đĩa. Tiến hành bỏ qua lệnh tạo mới để nạp thẳng cấu trúc bảng...");
                        try
                        {
                            await dbContext.Database.MigrateAsync();
                            logger.LogInformation("✅ [DEVOPS]: Đồng bộ dữ liệu Migration thành công sau khi xử lý xung đột!");
                            return;
                        }
                        catch (Exception migrationEx)
                        {
                            logger.LogError($"❌ [DEVOPS]: Lỗi nạp bảng Migration: {migrationEx.Message}");
                        }
                    }

                    // Các lỗi khác (Chưa mở cổng, đang boot mạng...) thì tiến hành đợi để thử lại
                    logger.LogWarning($"⏳ [DEVOPS]: SQL Server chưa sẵn sàng ở lần thử {i}/{maxRetries}. Đang đợi {delaySeconds}s... (Chi tiết lỗi: {ex.Message})");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            logger.LogError("❌ [DEVOPS]: Quá thời gian chờ kết nối SQL Server. Tiến trình triển khai thất bại.");
            throw new Exception("Hạ tầng SQL Server không phản hồi đúng hạn.");
        }
    }
}