using Domain.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Title
    {
        [Index(IsUnique = true)]
        public string? TitleId { get; set; }
        public string? TitleType { get; set; }
        public string? PrimaryTitle { get; set; }
        [BsonIgnore]
        public string? OriginalTitle { get; set; }
        [BsonIgnore]
        public bool IsAdult { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public int RuntimeMinutes { get; set; }
        public List<string>? Genres { get; set; }
    }
}
