using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Entry;
using Entities.Models;

ServiceProvider sp = new ServiceCollection()
    .AddLogging((loggingBuilder) => loggingBuilder
        .SetMinimumLevel(LogLevel.Trace)
        .AddConsole()
        )
    .BuildServiceProvider();

var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

var logLossGetter = new OddsGetter(loggerFactory);

// Get logger and run main
using (var scope = sp.CreateScope())
{
    ApiSettings oddsApiSettings = new ApiSettings();
    string? gamesConnectionString = Environment.GetEnvironmentVariable("NHL_DATABASE");
    oddsApiSettings.OddsApiKey = Environment.GetEnvironmentVariable("ODDS_API_KEY");

    if (gamesConnectionString == null)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.Local.json").Build();
        gamesConnectionString = config.GetConnectionString("NHL_DATABASE");        
    }
    if (oddsApiSettings.OddsApiKey == null)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.Local.json").Build();
        oddsApiSettings.OddsApiKey = config.GetValue<string>("ApiSettings:ODDS_API_KEY");
    }

    if (gamesConnectionString == null)
        throw new Exception("Connection String Null");
    if (oddsApiSettings == null)
        throw new Exception("Api Key Null");

    await logLossGetter.Main(gamesConnectionString, oddsApiSettings);
}