using Disqord;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using Disqord.Extensions.Interactivity;
using Disqord.Extensions.Interactivity.Menus;
using Disqord.Gateway;
using Disqord.Rest;
using Gabriel.Menus;

namespace Gabriel.Services;

public class GabrielService : DiscordBotService
{
    private readonly SearchService _searchService;
    private readonly DocumentationService _documentationService;

    public GabrielService(SearchService searchService, DocumentationService documentationService)
    {
        _searchService = searchService;
        _documentationService = documentationService;
    }

    protected override async ValueTask OnCommandNotFound(DiscordCommandContext context)
    {
        var type = _searchService.SearchTypes(context.Input).FirstOrDefault();
        if (type is null)
        {
            await context.Message.GetChannel().SendMessageAsync(new LocalMessage().WithContent("No matching type found"));
            return;
        }

        var view = new DocumentationView(_documentationService.GetDocumentation(type));
        await Bot.GetInteractivity().StartMenuAsync(context.ChannelId, new DefaultMenu(view));
    }
}
