using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.Business.Services
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm);
        Task<Tag?> GetTagByIdAsync(int id);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> UpdateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(int id);
        Task<bool> ValidateTagAsync(Tag tag);
    }
}
