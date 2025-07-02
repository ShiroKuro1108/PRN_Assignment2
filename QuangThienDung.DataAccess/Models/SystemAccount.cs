using System.ComponentModel.DataAnnotations;

namespace QuangThienDung.DataAccess.Models
{
    public class SystemAccount
    {
        [Key]
        public short AccountID { get; set; }

        [StringLength(100)]
        public string? AccountName { get; set; }

        [StringLength(70)]
        [EmailAddress]
        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; }

        [StringLength(70)]
        public string? AccountPassword { get; set; }

        // Navigation properties
        public virtual ICollection<NewsArticle> CreatedNewsArticles { get; set; } = new List<NewsArticle>();
        public virtual ICollection<NewsArticle> UpdatedNewsArticles { get; set; } = new List<NewsArticle>();
    }

    // Role constants
    public static class AccountRoles
    {
        public const int Staff = 1;
        public const int Lecturer = 2;
        // Admin role will be determined from appsettings.json
    }
}
