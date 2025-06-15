using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text;
using TinyCity.BookmarkEngines;
using TinyCity.Model;

namespace TinyCity.Commands
{
    public class ListCommandSettings : BaseSettings
    {
        [CommandOption("-e|--export")]
        [Description("Exports the results as 'exported-bookmarks.md' to the same directory as tinycity.")]
        [DefaultValue(false)]
        public bool Export { get; set; }

        [CommandOption("--export-format")]
        [Description("When exporting, sets the format of each link")]
        [DefaultValue("- [{name}]({url}) ({urlhost})")]
        public string ExportFormat { get; set; }
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
            var exportStringBuilder = new StringBuilder();
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

                    string exportLine = settings.ExportFormat
                                                .Replace("{name}", Markup.Escape(bookmarkName))
                                                .Replace("{url}", bookmarkUrl)
                                                .Replace("{urlhost}", urlHost);

                    exportStringBuilder.AppendLine(exportLine);
                }
            }

            if (settings.Export)
            {
                File.WriteAllText("exported-bookmarks.md", exportStringBuilder.ToString());
                AnsiConsole.MarkupLine($"[bold green]Exported to all bookmarks 'exported-bookmarks.md'[/].");
            }

            return 0;
        }
    }
}
