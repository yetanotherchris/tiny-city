using Spectre.Console;
using Spectre.Console.Cli;
using TinyCity.BookmarkEngines;
using TinyCity.Model;

namespace TinyCity.Commands
{
    public class ListCommandSettings : CommandSettings
    {
    }

    public class ListCommand : Command<ListCommandSettings>
    {
        private List<BookmarkNode> _combinedBookmarks;

        public ListCommand(BookmarkAggregator bookmarkAggregator)
        {
            _combinedBookmarks = bookmarkAggregator.AllBookmarks;
        }

        public override int Execute(CommandContext context, ListCommandSettings settings)
        {
            AnsiConsole.MarkupLine($"[bold turquoise2]{_combinedBookmarks.Count} unique bookmarks in total.[/]");

            foreach (var bookmark in _combinedBookmarks.OrderBy(x => x.Name))
            {
                if (!string.IsNullOrEmpty(bookmark.Url))
                {
                    string bookmarkUrl = Markup.Escape(bookmark.Url);
                    string bookmarkName = Markup.Escape(bookmark.Name);

                    string link = $"[link={bookmarkUrl}]{bookmarkName}[/]";
                    string urlHost = new Uri(bookmark.Url).Host;
                    AnsiConsole.MarkupLine($" • [bold chartreuse1]{link}[/] ({urlHost})");
                }
            }

            return 0;
        }
    }
}
