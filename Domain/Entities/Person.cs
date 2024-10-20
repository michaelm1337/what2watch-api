using Domain.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Person
    {
        [Index(IsUnique = true)]
        public string? PersonId { get; set; }
        public string? PrimaryName { get; set; }
        public int BirthYear { get; set; }
        public int DeathYear { get; set; }
        public List<string>? PrimaryProfessions { get; set; }
        public List<string>? KnownForTitles { get; set; }
    }
}
