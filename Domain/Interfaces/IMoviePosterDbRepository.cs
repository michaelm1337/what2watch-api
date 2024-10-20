namespace Domain.Interfaces
{
    public interface IMoviePosterDbRepository
    {
        Task InsertPosterAsync(string titleId, string fileName, Stream stream);
        Task<List<string?>> GetInsertedPosters();
    }
}
