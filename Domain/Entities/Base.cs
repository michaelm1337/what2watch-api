namespace Domain.Entities
{
    public class Base
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
    }
}
