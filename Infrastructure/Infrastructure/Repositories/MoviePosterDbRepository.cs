using Domain.Entities;
using Domain.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Infrastructure.Repositories
{
    public class MoviePosterDbRepository : IMoviePosterDbRepository
    {
        private IMongoCollection<PosterMetadata> _posters;
        private GridFSBucket _gridFsBucket;

        public MoviePosterDbRepository(IMongoDatabase db)
        {
            _posters = db.GetCollection<PosterMetadata>("posters");
            _gridFsBucket = new GridFSBucket(db);
        }

        public Task<List<string?>> GetInsertedPosters()
        {
            return Task.FromResult(_posters
                .AsQueryable()
                .Select(p => p.TitleId)
                .ToList());
        }

        public async Task InsertPosterAsync(string titleId, string fileName, Stream stream)
        {
            var exists = _posters.AsQueryable().FirstOrDefault(i => i.TitleId == titleId) != null;
            if (exists)
                return;

            var fileId = await _gridFsBucket.UploadFromStreamAsync(fileName, stream);

            var metadata = new PosterMetadata
            {
                TitleId = titleId,
                FileId = fileId,
                FileName = fileName,
                InsertDate = DateTime.Now
            };

            await _posters.InsertOneAsync(metadata);
        }
    }
}
