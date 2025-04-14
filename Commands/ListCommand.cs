using Spectre.Console;
using Spectre.Console.Cli;
using TinyCity.BookmarkEngines;
using TinyCity.Model;

namespace TinyCity.Commands
{
    public class ListSettings : CommandSettings
    {
    }

    public class ListCommand : Command<ListSettings>
    {
        private List<BookmarkNode> _combinedBookmarks;

        public ListCommand(ChromeBookmarks chromeBookmarks, MarkdownBookmarks markdownBookmarks)
        {
            _combinedBookmarks = new List<BookmarkNode>();
            _combinedBookmarks.AddRange(chromeBookmarks.FlattenedBookmarks);
            _combinedBookmarks.AddRange(markdownBookmarks.Bookmarks);
        }

        public override int Execute(CommandContext context, ListSettings settings)
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
