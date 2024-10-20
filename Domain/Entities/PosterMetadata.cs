using MongoDB.Bson;

namespace Domain.Entities
{
    public class PosterMetadata
    {
        public string? TitleId { get; set; }
        public ObjectId FileId { get; set; }
        public string? FileName { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
