using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics;
using TinyCity.BookmarkEngines;
using TinyCity.Model;

namespace TinyCity.Commands
{
    public class SearchCommandSettings : CommandSettings
    {
        [CommandOption("-l|--launch")]
        [Description("Launch the first bookmark found din your default browser. If no bookmarks are found, nothing will happen.")]
        [DefaultValue(false)]
        public bool Launch { get; set; }

        [CommandOption("-u|--urls")]
        [Description("Also search bookmark urls.")]
        [DefaultValue(false)]
        public bool SearchUrls { get; set; }

        [CommandArgument(0, "<query>")]
        [Description("The search term to look for in bookmarks. Enclose your search inside quotes, e.g. \"my search words\"")]
        public required string Query { get; set; }
    }

    public class SearchCommand : Command<SearchCommandSettings>
    {
        private List<BookmarkNode> _combinedBookmarks;

        public SearchCommand(BookmarkAggregator bookmarkAggregator)
        {
            _combinedBookmarks = bookmarkAggregator.AllBookmarks;
        }

        public override int Execute(CommandContext context, SearchCommandSettings settings)
        {
            var filteredBookmarks = Search(settings.Query, settings.SearchUrls);
            int count = filteredBookmarks.Count;
            if (count == 0)
            {
                AnsiConsole.MarkupLine($"[bold yellow]No bookmarks found for '{settings.Query}'.[/]");
                return 0;
            }

            AnsiConsole.MarkupLine($"[bold turquoise2]{count} bookmark(s) found for '{settings.Query}'.[/]");
            foreach (var bookmark in filteredBookmarks)
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

            if (settings.Launch)
            {
                var first = filteredBookmarks.FirstOrDefault();
                AnsiConsole.MarkupLine($"[bold green]Launching '{first.Name}'[/]...");

                var startInfo = new ProcessStartInfo
                {
                    FileName = first.Url,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }

            return 0;
        }

        private List<BookmarkNode> Search(string searchTerm, bool searchUrls)
        {
            searchTerm = searchTerm.ToLower();

            if (searchUrls)
            {
                return _combinedBookmarks
                      .Where(b => b.Name.ToLower().Contains(searchTerm) || (b.Url != null && b.Url.ToLower().Contains(searchTerm)))
                      .ToList();
            }
            else
            {
                return _combinedBookmarks
                       .Where(b => b.Name.ToLower().Contains(searchTerm))
                       .ToList();
            }
        }
    }
}
