using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Infrastructure.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly IMongoCollection<ContentEntity> _contents;

        public ContentRepository(IMongoDatabase db)
        {
            _contents = db.GetCollection<ContentEntity>("contents");
        }

        public async Task<bool> CreateContentAsync(ContentEntity content, CancellationToken cancellationToken)
        {
            await _contents.InsertOneAsync(content, null, cancellationToken);

            return true;
        }

        public async Task<bool> DeleteContentAsync(string id, CancellationToken cancellationToken)
        {
            var result = await _contents.DeleteOneAsync(content => content.Id == id, cancellationToken);

            return result.DeletedCount > 0;
        }

        public async Task<PagedResult<ContentEntity>> GetAllContentsAsync(int page, int pageSize, CancellationToken cancellationToken)
        {  
            var query = _contents
                .AsQueryable();

            var count = await query
                .LongCountAsync();

            var result = await query                
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ContentEntity>(result, page, pageSize, count);
        }

        public async Task<ContentEntity> GetContentAsync(string contentTitle, CancellationToken cancellationToken)
        {
            return await _contents.AsQueryable().FirstOrDefaultAsync(content => content.Title == contentTitle, cancellationToken);
        }
    }
}
