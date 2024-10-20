using Domain.Entities;
using Domain.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Infrastructure.Repositories
{
    public class ImdbRepository : IImdbRepository
    {
        private readonly IMongoCollection<Title> _imdbTitlesCollection;
        private readonly IMongoCollection<Crew> _crewsCollection;
        private readonly IMongoCollection<Episode> _episodesCollection;
        private readonly IMongoCollection<Rating> _ratingsCollection;
        private readonly IMongoCollection<Person> _personCollection;
        private readonly IMongoCollection<Principal> _principalCollection;

        public ImdbRepository(IMongoDatabase db)
        {
            _imdbTitlesCollection = db.GetCollection<Title>("titles").CreateIndexesAsync().Result;
            _crewsCollection = db.GetCollection<Crew>("crews").CreateIndexesAsync().Result;
            _episodesCollection = db.GetCollection<Episode>("episodes").CreateIndexesAsync().Result;
            _ratingsCollection = db.GetCollection<Rating>("ratings").CreateIndexesAsync().Result;
            _personCollection = db.GetCollection<Person>("persons").CreateIndexesAsync().Result;
            _principalCollection = db.GetCollection<Principal>("principals").CreateIndexesAsync().Result;
        }

        public Task<List<Title>> GetTitles(Func<Title, bool> func)
        {
            return Task.FromResult(_imdbTitlesCollection
                .AsQueryable()
                .Where(func)
                .ToList());
        }

        public async Task InsertCrews(List<Crew> crews)
        {
            var ids = crews.Select(i => i.TitleId).ToList();

            List<string?> inserted = _crewsCollection
                .AsQueryable()
                .Where(i => ids.Contains(i.TitleId))
                .Select(i => i.TitleId)
                .ToList();

            var toInsert = crews
                .ExceptBy(inserted, crew => crew.TitleId)
                .ToList();

            if (toInsert.Any())
            {
                await _crewsCollection.InsertManyAsync(toInsert);
            }
        }

        public async Task InsertEpisodes(List<Episode> episodes)
        {
            var ids = episodes.Select(i => i.TitleId).ToList();

            List<string?> inserted = _episodesCollection
                .AsQueryable()
                .Where(i => ids.Contains(i.TitleId))
                .Select(i => i.TitleId)
                .ToList();

            var toInsert = episodes
                .ExceptBy(inserted, episode => episode.TitleId)
                .ToList();

            if (toInsert.Any())
            {
                await _episodesCollection.InsertManyAsync(toInsert);
            }
        }

        public async Task InsertRatings(List<Rating> ratings)
        {
            var ids = ratings.Select(i => i.TitleId).ToList();

            List<string?> inserted = _ratingsCollection
                .AsQueryable()
                .Where(i => ids.Contains(i.TitleId))
                .Select(i => i.TitleId)
                .ToList();

            var toInsert = ratings
                .ExceptBy(inserted, rating => rating.TitleId)
                .ToList();

            var toUpdateLst = ratings
                .IntersectBy(inserted, rating => rating.TitleId)
                .ToList();

            if (toInsert.Any())
            {
                await _ratingsCollection.InsertManyAsync(toInsert);
            }

            var bulkOperations = new List<WriteModel<Rating>>();

            foreach (var toUpdate in toUpdateLst)
            {
                var filter = Builders<Rating>.Filter.Eq(doc => doc.TitleId, toUpdate.TitleId);
                var update = Builders<Rating>.Update
                    .Set(i => i.NumVotes, toUpdate.NumVotes)
                    .Set(i => i.AverageRating, toUpdate.AverageRating);

                var updateOne = new UpdateOneModel<Rating>(filter, update);
                bulkOperations.Add(updateOne);
            }
            if (bulkOperations.Any())
                await _ratingsCollection.BulkWriteAsync(bulkOperations);
        }

        public async Task InsertTitles(List<Title> titles)
        {
            var ids = titles.Select(i => i.TitleId).ToList();

            List<string?> inserted = _imdbTitlesCollection
                .AsQueryable()
                .Where(i => ids.Contains(i.TitleId))
                .Select(i => i.TitleId)
                .ToList();

            var toInsert = titles
                .ExceptBy(inserted, title => title.TitleId)
                .ToList();

            if (toInsert.Any())
            {
                await _imdbTitlesCollection.InsertManyAsync(toInsert);
            }
        }

        public async Task InsertPrincipals(List<Principal> principals)
        {
            var ids = principals.Select(i => i.TitleId).ToList();

            List<string?> inserted = _principalCollection
                .AsQueryable()
                .Where(i => ids.Contains(i.TitleId))
                .Select(i => i.TitleId)
                .ToList();

            var toInsert = principals
                .ExceptBy(inserted, title => title.TitleId)
                .ToList();

            if (toInsert.Any())
            {
                await _principalCollection.InsertManyAsync(toInsert);
            }
        }

        public async Task InsertPersons(List<Person> persons)
        {
            var ids = persons.Select(i => i.PersonId).ToList();

            List<string?> inserted = _personCollection
                .AsQueryable()
                .Where(i => ids.Contains(i.PersonId))
                .Select(i => i.PersonId)
                .ToList();

            var toInsert = persons
                .ExceptBy(inserted, rating => rating.PersonId)
                .ToList();

            var toUpdateLst = persons
                .IntersectBy(inserted, rating => rating.PersonId)
                .ToList();

            if (toInsert.Any())
            {
                await _personCollection.InsertManyAsync(toInsert);
            }

            var bulkOperations = new List<WriteModel<Person>>();

            foreach (var toUpdate in toUpdateLst)
            {
                var filter = Builders<Person>.Filter.Eq(doc => doc.PersonId, toUpdate.PersonId);
                var update = Builders<Person>.Update
                    .Set(i => i.DeathYear, toUpdate.DeathYear)
                    .Set(i => i.PrimaryProfessions, toUpdate.PrimaryProfessions)
                    .Set(i => i.KnownForTitles, toUpdate.KnownForTitles);

                var updateOne = new UpdateOneModel<Person>(filter, update);
                bulkOperations.Add(updateOne);
            }
            if (bulkOperations.Any())
                await _personCollection.BulkWriteAsync(bulkOperations);
        }
    }
}
