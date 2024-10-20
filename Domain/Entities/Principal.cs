using Domain.Attributes;

namespace Domain.Entities
{
    public class Principal
    {
        [Index(IsUnique = true)]
        public string? TitleId { get; set; }
        public int Ordering { get; set; }
        [Index]
        public string? PersonId { get; set; }
        public string? Category { get; set; }
        public string? Job { get; set; }
        public string? Characters { get; set; }
    }
}
