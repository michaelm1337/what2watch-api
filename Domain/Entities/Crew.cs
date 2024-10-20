using Domain.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Crew
    {
        [Index(IsUnique = true)]
        public string? TitleId { get; set; }
        public List<string>? Directors { get; set; }
        public List<string>? Writers { get; set; }
    }
}
