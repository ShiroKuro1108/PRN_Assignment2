using System.ComponentModel.DataAnnotations;

namespace QuangThienDung.DataAccess.Models
{
    public class Tag
    {
        [Key]
        public int TagID { get; set; }

        [StringLength(50)]
        public string? TagName { get; set; }

        [StringLength(400)]
        public string? Note { get; set; }

        // Navigation properties
        public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}
