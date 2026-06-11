using Football247.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football247.Models.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; } // Khóa ngoại liên kết tới ApplicationUser
        public string Token { get; set; } // Chuỗi refresh token
        public bool IsUsed { get; set; } // Đã được sử dụng chưa
        public DateTime ExpiryDate { get; set; }


        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
