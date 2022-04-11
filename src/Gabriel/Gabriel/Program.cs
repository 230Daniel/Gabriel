using Disqord;
using Disqord.Bot.Hosting;
using Gabriel.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Gabriel;

public class Program
{
    public static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
            .UseSystemd()
            .UseSerilog()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<AssemblyService>();
                services.AddSingleton<SearchService>();
                services.AddSingleton<DocumentationService>();
            })
            .ConfigureDiscordBot((context, bot) =>
            {
                bot.Token = context.Configuration["Token"];
                bot.Prefixes = new[] { context.Configuration["Prefix"] };
            })
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(host.Services.GetRequiredService<IConfiguration>())
            .CreateLogger();

        try
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Fatal exception");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
