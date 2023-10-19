using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Entry;

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
    string? gamesConnectionString = Environment.GetEnvironmentVariable("NHL_DATABASE");

    if (gamesConnectionString == null)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.Local.json").Build();
        gamesConnectionString = config.GetConnectionString("NHL_DATABASE");
    }
    if (gamesConnectionString == null)
        throw new Exception("Connection String Null");

    await logLossGetter.Main(gamesConnectionString);
}