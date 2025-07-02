using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.DataAccess.Repository
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm);
        Task UpdateAsync(Tag tag);
    }
}
