using Domain.Attributes;
using System.Reflection;

namespace MongoDB.Driver
{
    public static class MongoExtensions
    {
        public static async Task<IMongoCollection<T>> CreateIndexesAsync<T>(this IMongoCollection<T> collection)
        {
            var indexModels = new List<CreateIndexModel<T>>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var indexAttribute = property.GetCustomAttribute<IndexAttribute>();
                if (indexAttribute != null)
                {
                    var indexKeysDefinition = Builders<T>.IndexKeys.Ascending(property.Name);
                    var indexOptions = new CreateIndexOptions { Unique = indexAttribute.IsUnique };
                    var indexModel = new CreateIndexModel<T>(indexKeysDefinition, indexOptions);
                    indexModels.Add(indexModel);
                }
            }

            if (indexModels.Count > 0)
            {
                await collection.Indexes.CreateManyAsync(indexModels);
            }

            return collection;
        }
    }
}
