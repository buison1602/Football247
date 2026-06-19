using System.Text.Json.Serialization;

namespace Football247.Domain.Models.CommandModels.UserCmdModel
{
    public class UpdateUserAdminCommandModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        // === Basic Info (Thông tin cơ bản) ===
        /// <summary>Tên đăng nhập (min 6 chars)</summary>
        public string? Name { get; set; }

        /// <summary>Email</summary>
        public string? Email { get; set; }

        /// <summary>Số điện thoại</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Hình đại diện</summary>
        public string? AvatarUrl { get; set; }

        // === Notification Preferences ===
        /// <summary>Nhận thông báo trong ứng dụng?</summary>
        public bool? ReceiveInAppNotifications { get; set; }

        /// <summary>Nhận thông báo qua email?</summary>
        public bool? ReceiveEmailNotifications { get; set; }

        // === Gamification (Quản lý điểm thưởng) ===
        /// <summary>Điểm thưởng của user</summary>
        public int? Points { get; set; }

        /// <summary>Số lần quay vòng quay</summary>
        public int? SpinCount { get; set; }

        // === Role Management (Quản lý vai trò) ===
        /// <summary>
        /// Vai trò/Quyền hạn của tài khoản
        /// Giá trị hợp lệ: "Admin", "Member", "User"
        /// Mục đích: Thay đổi cấp bậc quyền hạn của tài khoản
        /// Ví dụ: nâng cấp User lên Member để viết bài, hoặc lên Admin để quản trị hệ thống
        /// </summary>
        public string? Role { get; set; }

        // === Security & Account Status ===
        /// <summary>Cho phép khóa tài khoản?</summary>
        public bool? LockoutEnabled { get; set; }

        /// <summary>Thời gian hết khóa tài khoản (null = không khóa)</summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>Email đã được xác nhận?</summary>
        public bool? EmailConfirmed { get; set; }

        /// <summary>Số điện thoại đã được xác nhận?</summary>
        public bool? PhoneNumberConfirmed { get; set; }

        /// <summary>Bật xác thực 2 yếu tố?</summary>
        public bool? TwoFactorEnabled { get; set; }

        /// <summary>Reset số lần đăng nhập thất bại</summary>
        public bool? ResetAccessFailedCount { get; set; }
    }
}