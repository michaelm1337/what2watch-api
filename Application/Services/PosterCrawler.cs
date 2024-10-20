using Application.WebRequests;
using Domain.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class PosterCrawler : IHostedService
    {
        private readonly IMoviePosterDbRepository posterRepository;
        private readonly IImdbRepository imdbRepository;
        private readonly ILogger<PosterCrawler> logger;
        private readonly HtmlDocument html = new();
        private readonly HttpClient _client;
        private readonly Random random = new();
        private readonly Random userAgentRandom = new();

        private readonly List<string> userAgents = ["Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:124.0) Gecko/20100101 Firefox/124.0",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.2420.81",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 OPR/109.0.0.0",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 14.4; rv:124.0) Gecko/20100101 Firefox/124.0",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 14_4_1) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.4.1 Safari/605.1.15",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 14_4_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 OPR/109.0.0.0",
"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36",
"Mozilla/5.0 (X11; Linux i686; rv:124.0) Gecko/20100101 Firefox/124.0"];

        public PosterCrawler(IMoviePosterDbRepository posterRepository, IImdbRepository imdbRepository, ILogger<PosterCrawler> logger)
        {
            var handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = new(),
                AllowAutoRedirect = true
            };

            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            this.posterRepository = posterRepository;
            this.imdbRepository = imdbRepository;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var inserted = await posterRepository.GetInsertedPosters();

            var titlesToInsert = (await imdbRepository
                .GetTitles(t => !inserted.Contains(t.TitleId)))
                .ToList();

            foreach (var title in titlesToInsert)
            {
                try
                {
                    if (!string.IsNullOrEmpty(title.PrimaryTitle) && !string.IsNullOrEmpty(title.TitleId))
                    {
                        await UploadPosterByTitleAsync(title.PrimaryTitle, title.TitleId);
                    }
                }
                catch (Exception ex)
                {
                    Thread.Sleep(30000);
                    logger.LogError("Upload process for title {titleId} failed -> {Message}", title.TitleId, ex.Message);
                }
                finally
                {
                    int randomUserAgent = userAgentRandom.Next(1, userAgents.Count - 1);
                    string userAgent = userAgents[randomUserAgent];
                    
                    _client.DefaultRequestHeaders.Clear();
                    _client.DefaultRequestHeaders.Add("User-Agent", userAgent);

                    int randomDelay = random.Next(4000, 11000);
                    await Task.Delay(randomDelay, cancellationToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task UploadPosterByTitleAsync(string titleName, string titleId)
        {
            var response = await _client.GetStringAsync($"https://www.google.com/search?tbm=isch&q={Uri.EscapeDataString($"https://www.imdb.com/title/{titleId}/")}");
            html.LoadHtml(response);

            var stringSplit = response
                .Split("[")
                .Where(i => i.Contains("/m.media-amazon.com/images/"))
                .FirstOrDefault();

            if (string.IsNullOrEmpty(stringSplit))
                throw new Exception("Couldn't extract the high resolution url from the html element attribute value");

            string pattern = @"\""((?:https?://|www\.)[^\""]+)\""";

            Match match = Regex.Match(stringSplit, pattern);

            string highResImageUrl = "";

            if (match.Success)
                highResImageUrl = match.Groups[1].Value;
            else
                throw new Exception($"High res image not found. TitleId: {titleId}");

            if (string.IsNullOrEmpty(highResImageUrl))
                throw new Exception($"High res image not found. TitleId: {titleId}");

            using var stream = await _client.GetStreamAsync(highResImageUrl);
            await posterRepository.InsertPosterAsync(titleId, titleName + titleId, stream);
        }
    }
}
