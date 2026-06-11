namespace Football247.Domain.Models.EntityModels.DTOs.SendEmail
{
    public class SendEmailDto
    {
        // Danh sách các email người nhận
        public IList<string> ToEmails { get; set; } = new List<string>();

        // Tiêu đề email.
        public string? Subject { get; set; }

        // Nội dung email, văn bản thuần túy hoặc HTML
        public string? Content { get; set; }
    }
}
