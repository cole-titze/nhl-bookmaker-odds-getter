using DataAccess;
using DataAccess.GameOddsRepository;
using BusinessLogic.GameOddsGetter;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Services.RequestMaker;
using Services.NhlData;
using Entities.Models;

namespace Entry
{
    public class OddsGetter
    {
        private readonly ILogger<OddsGetter> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public OddsGetter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<OddsGetter>();
        }
        public async Task Main(string gamesConnectionString, ApiSettings apiSettings)
        {
            // Run Data Collection
            var watch = Stopwatch.StartNew();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            _logger.LogTrace("Starting Odds Getting");
            var requestMaker = new RequestMaker(new HttpClientWrapper());
            var gameDbContext = new GameDbContext(gamesConnectionString);
            var gameOddsRepo = new GameOddsRepository(gameDbContext);
            var nhlGameGetter = new NhlGameOddsGetter(requestMaker, apiSettings, _loggerFactory);
            var gameOddsGetter = new GameOddsGetter(gameOddsRepo, nhlGameGetter, _loggerFactory);

            var gameOdds = await gameOddsGetter.GetGameOdds();
            await gameOddsRepo.UpdateGameOdds(gameOdds);

            watch.Stop();
            var elapsedTime = watch.Elapsed;
            var minutes = elapsedTime.TotalMinutes.ToString();
            _logger.LogTrace("Completed Getting Odds in " + minutes + " minutes");
        }
    }
}