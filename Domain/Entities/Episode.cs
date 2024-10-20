using Domain.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Episode
    {
        public string? EpisodeId { get; set; }
        [Index(IsUnique = true)]
        public string? TitleId { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
    }
}
