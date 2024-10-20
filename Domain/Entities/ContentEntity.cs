using Domain.ValueObjects;

namespace Domain.Entities
{
    public class ContentEntity : Base
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required Poster Poster { get; set; }
    }
}
