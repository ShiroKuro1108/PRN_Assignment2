using QuangThienDung.DataAccess.Models;
using QuangThienDung.DataAccess.Repository;

namespace QuangThienDung.Business.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            try
            {
                if (!await ValidateTagAsync(tag))
                    return false;

                await _unitOfWork.Tag.AddAsync(tag);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            try
            {
                var tag = await _unitOfWork.Tag.GetAsync(t => t.TagID == id);
                if (tag == null)
                    return false;

                _unitOfWork.Tag.Remove(tag);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _unitOfWork.Tag.GetAllAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            return await _unitOfWork.Tag.GetAsync(t => t.TagID == id);
        }

        public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm)
        {
            return await _unitOfWork.Tag.SearchTagsAsync(searchTerm);
        }

        public async Task<bool> UpdateTagAsync(Tag tag)
        {
            try
            {
                if (!await ValidateTagAsync(tag))
                    return false;

                _unitOfWork.Tag.Update(tag);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> ValidateTagAsync(Tag tag)
        {
            if (string.IsNullOrWhiteSpace(tag.TagName))
                return Task.FromResult(false);

            return Task.FromResult(true);
        }
    }
}
