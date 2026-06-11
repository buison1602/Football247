using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.CommandModels.SendEmailCmdModel
{
    public class SendEmailCommandModel
    {
        // Danh sách email nhận email, có thể gửi tới nhiều email cùng lúc
        public IList<string> ToEmails { get; set; } = new List<string>();

        // Tiêu đề email.
        public string? Subject { get; set; }

        // Nội dung email, văn bản thuần túy hoặc HTML
        public string? Content { get; set; }

        // Mẫu email, nếu có thể sử dụng các mẫu email đã định nghĩa trước để gửi email nhanh chóng và nhất quán.
        public EnumSenderTemplate? Template { get; set; }

        public object? Params { get; set; }

    }
}
