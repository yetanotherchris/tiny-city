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

        public ListCommand(ChromeBookmarks chromeBookmarks, MarkdownBookmarks markdownBookmarks)
        {
            _combinedBookmarks = [.. chromeBookmarks.FlattenedBookmarks, .. markdownBookmarks.Bookmarks];
        }

        public override int Execute(CommandContext context, ListCommandSettings settings)
        {
            AnsiConsole.MarkupLine($"[bold green]{_combinedBookmarks.Count} bookmarks[/]");

            foreach (var bookmark in _combinedBookmarks.OrderBy(x => x.Name))
            {
                if (!string.IsNullOrEmpty(bookmark.Url))
                {
                    string link = $"[link={bookmark.Url}]{bookmark.Name}[/]";
                    string urlHost = new Uri(bookmark.Url).Host;
                    AnsiConsole.MarkupLine($"[bold chartreuse1]{link}[/] ({urlHost})");
                }
            }

            return 0;
        }
    }
}
