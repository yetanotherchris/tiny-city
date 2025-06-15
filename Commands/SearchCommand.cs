using Spectre.Console;
using Spectre.Console.Cli;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using TinyCity.BookmarkEngines;
using TinyCity.Model;

namespace TinyCity.Commands
{
    public class SearchCommandSettings : BaseSettings
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

        [CommandOption("-e|--export")]
        [Description("Exports the results as 'exported-bookmarks.md' to the same directory as tinycity.")]
        [DefaultValue(false)]
        public bool Export { get; set; }

        [CommandOption("--export-format")]
        [Description("When exporting, sets the format of each link")]
        [DefaultValue("- [{name}]({url}) ({urlhost})")]
        public string ExportFormat { get; set; }
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
            var exportStringBuilder = new StringBuilder();
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

                    string exportLine = settings.ExportFormat
                                                .Replace("{name}", Markup.Escape(bookmarkName))
                                                .Replace("{url}", bookmarkUrl)
                                                .Replace("{urlhost}", urlHost);

                    exportStringBuilder.AppendLine(exportLine);
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
            else if (settings.Export)
            {
                File.WriteAllText("exported-bookmarks.md", exportStringBuilder.ToString());
                AnsiConsole.MarkupLine($"[bold green]Exported search results to 'exported-bookmarks.md'[/].");
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
                      .OrderBy(x => x.Name)
                      .ToList();
            }
            else
            {
                return _combinedBookmarks
                       .Where(b => b.Name.ToLower().Contains(searchTerm))
                       .OrderBy(x => x.Name)
                       .ToList();
            }
        }
    }
}
