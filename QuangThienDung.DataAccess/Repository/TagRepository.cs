using Microsoft.EntityFrameworkCore;
using QuangThienDung.DataAccess.Data;
using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.DataAccess.Repository
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        private readonly FUNewsManagementContext _context;

        public TagRepository(FUNewsManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm)
        {
            return await _context.Tags
                .Where(t => t.TagName!.Contains(searchTerm) || 
                           t.Note!.Contains(searchTerm))
                .OrderBy(t => t.TagName)
                .ToListAsync();
        }

        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }
    }
}
