using Domain.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Rating
    {
        [Index(IsUnique = true)]
        public string? TitleId { get; set; }
        public string? AverageRating { get; set; }
        public int NumVotes { get; set; }
    }
}
