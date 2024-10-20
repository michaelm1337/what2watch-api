using Domain.Entities;
using Domain.Shared;

namespace Domain.Interfaces
{
    public interface IContentRepository
    {
        Task<ContentEntity> GetContentAsync(string contentTitle, CancellationToken cancellationToken);
        Task<PagedResult<ContentEntity>> GetAllContentsAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<bool> DeleteContentAsync(string id, CancellationToken cancellationToken);
        Task<bool> CreateContentAsync(ContentEntity content, CancellationToken cancellationToken);
    }
}
