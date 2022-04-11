using System.Text;
using Disqord;
using Gabriel.Services;

namespace Gabriel.Extensions;

public static class TypeDocumentationExtensions
{
    public static List<LocalEmbed> BuildEmbeds(this TypeDocumentation typeDocumentation)
    {
        var embeds = new List<LocalEmbed>();

        var properties = SplitIntoPages(typeDocumentation.Properties, 4096 - typeDocumentation.Summary.Length - 8, 4096 - 8);
        //var methods = SplitIntoPages(typeDocumentation.Methods, 4096 - )
        embeds.Add(new LocalEmbed()
            .WithTitle(typeDocumentation.Name)
            .WithDescription(typeDocumentation.Summary + properties.First()));

        return embeds;
    }

    private static List<string> SplitIntoPages(List<string> items, int maxFirstPageSize, int maxPageSize)
    {
        var pages = new List<string>();
        var currentPage = new StringBuilder();
        var firstPage = true;
        foreach (var item in items)
        {
            if (currentPage.Length + item.Length + 1 <= (firstPage ? maxFirstPageSize : maxPageSize))
            {
                currentPage.Append(item);
            }
            else
            {
                pages.Add(currentPage.ToString());
                currentPage = new StringBuilder(item);
                firstPage = false;
            }

            currentPage.Append('\n');
        }

        return pages;
    }
}
