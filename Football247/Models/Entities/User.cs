using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class User
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        // Các bài viết do user tạo
        public ICollection<Article> Articles { get; set; }
    }
}
