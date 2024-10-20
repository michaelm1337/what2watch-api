using Application.Helpers;
using Application.WebRequests;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection;

namespace Application.Services
{
    public class ImdbCrawler : IHostedService
    {
        private readonly ILogger<ImdbCrawler> _logger;
        private readonly IImdbFiles _imdbFilesRepository;
        private readonly IImdbRepository _imdbRepository;

        private bool IsMainTitleThreadRunning = false;

        private readonly List<string> _zipFiles = [
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "1title.basics.tsv.gz"),
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "2title.crew.tsv.gz"),
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "3title.episode.tsv.gz"),
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "4title.principals.tsv.gz"),
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "5name.basics.tsv.gz"),
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "6title.ratings.tsv.gz")
        ];

        public ImdbCrawler(IImdbFiles imdbFilesRepository, ILogger<ImdbCrawler> logger, IImdbRepository imdbRepository)
        {
            _imdbFilesRepository = imdbFilesRepository;
            _logger = logger;
            _imdbRepository = imdbRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                ConcurrentBag<string> uncompressedFiles = [];

                ParallelOptions parallelOptions = new()
                {
                    MaxDegreeOfParallelism = 3
                };

                await Parallel.ForEachAsync(_zipFiles, parallelOptions, async (zipFile, cancellationToken) =>
                {
                    string uncompressedFile = zipFile.Replace(".gz", "");

                    await DownloadFilesAsync(zipFile);
                    await UnzipGzFile(zipFile, uncompressedFile);

                    uncompressedFiles.Add(uncompressedFile);
                    if (File.Exists(zipFile))
                        File.Delete(zipFile);
                });

                var tasks = new List<Task>();

                uncompressedFiles = [.. uncompressedFiles.OrderByDescending(o => o)];

                foreach (var uncompressedFile in uncompressedFiles)
                {
                    tasks.Add(new Task(() =>
                    {
                        Load(uncompressedFile).Wait();
                    }, cancellationToken));
                }

                tasks.ForEach(task =>
                {
                    task.Start();
                    Thread.Sleep(2000);
                });

                await Task.WhenAll([.. tasks]);

                foreach (var file in uncompressedFiles.Select(f => new FileInfo(f)))
                    if (file.Exists)
                        file.Delete();
            }
            catch (Exception ex)
            {
                _logger.LogError("Imdb tsv files loading process failed -> {Message}", ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task DownloadFilesAsync(string zipFile)
        {
            Stream stream = Stream.Null;

            if (zipFile.Contains("1title.basics.tsv"))
                stream = await _imdbFilesRepository.GetTitleBasics();

            if (zipFile.Contains("2title.crew.tsv"))
                stream = await _imdbFilesRepository.GetTitleCrew();

            if (zipFile.Contains("3title.episode.tsv"))
                stream = await _imdbFilesRepository.GetTitleEpisodes();

            if (zipFile.Contains("4title.principals.tsv"))
                stream = await _imdbFilesRepository.GetPrincipals();

            if (zipFile.Contains("5name.basics.tsv"))
                stream = await _imdbFilesRepository.GetPersons();

            if (zipFile.Contains("6title.ratings.tsv"))
                stream = await _imdbFilesRepository.GetTitleRatings();

            using (Stream contentStream = stream,
                    fileStream = new FileStream(zipFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                await contentStream.CopyToAsync(fileStream);
            }
        }

        private static async Task UnzipGzFile(string zipFile, string file)
        {
            using FileStream originalFileStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read);
            using FileStream decompressedFileStream = new FileStream(file, FileMode.Create, FileAccess.Write);
            using GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress);
            await decompressionStream.CopyToAsync(decompressedFileStream);
        }

        private async Task Load(string file)
        {
            FileInfo fileInfo = new(file);

            if (fileInfo.Exists)
            {
                if (fileInfo.Name == "1title.basics.tsv")
                {
                    IsMainTitleThreadRunning = true;
                    await fileInfo.ConvertAndInsert<Title>(_imdbRepository, []);
                    IsMainTitleThreadRunning = false;
                }

                while (IsMainTitleThreadRunning) Thread.Sleep(3000);

                List<string> insertedTitles = (await _imdbRepository
                    .GetTitles(title => true))
                    .Select(t => t.TitleId ?? "")
                    .ToList();

                if (fileInfo.Name == "6title.ratings.tsv")
                    await fileInfo.ConvertAndInsert<Rating>(_imdbRepository, insertedTitles);

                if (fileInfo.Name == "2title.crew.tsv")
                    await fileInfo.ConvertAndInsert<Crew>(_imdbRepository, insertedTitles);

                if (fileInfo.Name == "3title.episode.tsv")
                    await fileInfo.ConvertAndInsert<Episode>(_imdbRepository, insertedTitles);

                if (fileInfo.Name == "4title.principals.tsv")
                    await fileInfo.ConvertAndInsert<Principal>(_imdbRepository, insertedTitles);

                if (fileInfo.Name == "5name.basics.tsv")
                    await fileInfo.ConvertAndInsert<Person>(_imdbRepository, insertedTitles);
            }
        }
    }
}
