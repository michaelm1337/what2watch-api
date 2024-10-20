using Domain.Entities;
using Domain.Interfaces;

namespace Application.Helpers
{
    public static class FileExtensions
    {
        private static readonly List<Type> allowedTypes = [typeof(Title), typeof(Crew), typeof(Episode), typeof(Rating), typeof(Person), typeof(Principal)];
        private static readonly int batchSize = 10000;

        public static async Task ConvertAndInsert<T>(this FileInfo file, IImdbRepository repository, List<string> insertedTitles)
        {
            using FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            using BufferedStream bs = new(fs);
            using StreamReader sr = new(bs);
            List<string> lines = [];
            var type = typeof(T);
            var currentType = allowedTypes
                        .FirstOrDefault(t => t.Name == type.Name);

            if (currentType == null)
                throw new InvalidDataException("Unknown type cannot be processed");

            string? readingLine = await sr.ReadLineAsync();
            while ((readingLine = await sr.ReadLineAsync()) != null)
            {
                lines.Add(readingLine);

                if (lines.Count >= batchSize)
                {
                    var batch = GetConvertedBatch<T>(type, lines, insertedTitles);

                    await InsertLines(currentType, batch, repository);

                    lines.Clear();
                    batch.Clear();
                }
            }

            if (lines.Count > 0)
            {
                var batch = GetConvertedBatch<T>(type, lines, insertedTitles);
                if (currentType != null)
                {
                    await InsertLines(currentType, batch, repository);
                }
            }
        }

        private static List<T> GetConvertedBatch<T>(Type type, List<string> lines, List<string> insertedTitles)
        {
            List<T> batch = [];

            var properties = type.GetProperties();

            foreach (var line in lines)
            {
                var obj = Activator.CreateInstance(type);

                string[] lineValues = [.. line.Split("\t")];

                for (int i = 0; i < properties.Length; i++)
                {
                    var propType = properties[i].PropertyType;

                    if (propType == typeof(int))
                    {
                        int value = TryGetInt(lineValues, i);
                        properties[i].SetValue(obj, value);
                    }
                    else if (propType == typeof(bool))
                    {
                        bool value = bool.Parse((TryGetString(lineValues, i) == "1" ? "True" : "False") ?? "false");
                        properties[i].SetValue(obj, value);
                    }
                    else if (propType == typeof(List<string>))
                    {
                        List<string> values = [.. (TryGetString(lineValues, i) ?? "").Split(',')];
                        properties[i].SetValue(obj, values);
                    }
                    else
                    {
                        string? value = TryGetString(lineValues, i);

                        if (!string.IsNullOrEmpty(value))
                        {
                            properties[i].SetValue(obj, value);
                        }
                    }
                }

                if (obj != null)
                {
                    if (type == typeof(Title))
                    {
                        var title = (Title)obj;
                        if (title.IsAdult ||
                            title.StartYear <= 1960 ||
                            (title.TitleType != "movie" && title.TitleType != "tvSeries"))
                            continue;
                    }

                    batch.Add((T)obj);
                }
            }

            if (type != typeof(Title) && type != typeof(Person))
            {
                if (insertedTitles != null && insertedTitles.Any())
                    batch = batch
                        .IntersectBy(insertedTitles, b => ((dynamic)b).TitleId)
                        .ToList();
            }

            return batch;
        }

        private static async Task InsertLines<T>(Type currentType, List<T> batch, IImdbRepository repository)
        {
            if (currentType.Name == "Title")
            {
                var titles = ((IEnumerable<Title>)batch).ToList();
                await repository.InsertTitles(titles);
            }

            if (currentType.Name == "Crew")
            {
                var crews = ((IEnumerable<Crew>)batch).ToList();
                await repository.InsertCrews(crews);
            }

            if (currentType.Name == "Episode")
            {
                var episodes = ((IEnumerable<Episode>)batch).ToList();
                await repository.InsertEpisodes(episodes);
            }

            if (currentType.Name == "Rating")
            {
                var ratings = ((IEnumerable<Rating>)batch).ToList();
                await repository.InsertRatings(ratings);
            }

            if (currentType.Name == "Principal")
            {
                var principals = ((IEnumerable<Principal>)batch).ToList();
                await repository.InsertPrincipals(principals);
            }

            if (currentType.Name == "Person")
            {
                var persons = ((IEnumerable<Person>)batch).ToList();
                await repository.InsertPersons(persons);
            }
        }

        private static string? TryGetString(string[]? array, int index)
        {
            if (array is null)
                return null;

            return array[index]?.Replace("\n", "").Replace(@"\N", "");
        }

        private static int TryGetInt(string[]? array, int index)
        {
            if (array is null || array[index] is null)
                return 0;

            if (int.TryParse(array[index], null, out int result))
                return result;

            return 0;
        }
    }
}
