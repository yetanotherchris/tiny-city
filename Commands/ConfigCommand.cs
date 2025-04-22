using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Linq;
using TinyCity.BookmarkEngines;
using TinyCity.Model;

namespace TinyCity.Commands
{

    public class ConfigCommandSettings : CommandSettings
    {
        [CommandOption("-a|--add-markdown-file")]
        [Description("Adds a markdown file to scan for links.")]
        public string AddMarkdownFile { get; set; }

        [CommandOption("-r|--remove-markdown-file")]
        [Description("Remove a markdown file from the config.")]
        public string RemoveMarkdownFile { get; set; }

        [CommandOption("-s|--show")]
        [Description("Show the current configuration.")]
        public bool Show { get; set; }

        [CommandOption("-b|--set-browser")]
        [Description("Set the browser type to search bookmarks from. Valid values are: chrome, opera, brave, edge.")]
        public string Browser { get; set; }

        [CommandOption("-h|--html-bookmark-file")]
        [Description("Sets a HTML bookmark file to scan for links.")]
        public string HtmlBookmarkFile { get; set; }
    }

    public class ConfigCommand : Command<ConfigCommandSettings>
    {
        private readonly TinyCitySettings _tinyCitySettings;
        private readonly BookmarkAggregator _bookmarkAggregator;

        public ConfigCommand(TinyCitySettings settings, BookmarkAggregator bookmarkAggregator)
        {
            _tinyCitySettings = settings;
            _bookmarkAggregator = bookmarkAggregator;
        }

        public override int Execute(CommandContext context, ConfigCommandSettings settings)
        {
            if (settings.Show)
            {
                ShowConfiguration();
            }
            else if (!string.IsNullOrEmpty(settings.AddMarkdownFile))
            {
                AddMarkdownFile(settings.AddMarkdownFile);
            }
            else if (!string.IsNullOrEmpty(settings.RemoveMarkdownFile))
            {
                RemoveMarkdownFile(settings.RemoveMarkdownFile);
            }
            else if (!string.IsNullOrEmpty(settings.Browser))
            {
                SetBrowser(settings.Browser);
            }
            else if (!string.IsNullOrEmpty(settings.HtmlBookmarkFile))
            {
                SetHtmlBookmarkFile(settings.HtmlBookmarkFile);
            }

            return 0;
        }

        private void ShowConfiguration()
        {
            AnsiConsole.MarkupLine($"[deepskyblue1]Bookmark sources ({_bookmarkAggregator.AllBookmarks.Count} unique bookmarks in total):[/]");
            _bookmarkAggregator.WriteLoadedLog();

            AnsiConsole.MarkupLine($"[deepskyblue1]Configuration ('{TinyCitySettings.GetConfigFilePath()}'):[/]");
            AnsiConsole.MarkupLine($" - Home Directory: {_tinyCitySettings.ApplicationConfigDirectory}.");
            AnsiConsole.MarkupLine($" - Browser path: {_tinyCitySettings.BrowserPath}.");

            string htmlFilePath = _tinyCitySettings.HtmlBookmarksFile ?? "(none)";
            AnsiConsole.MarkupLine($" - HTML bookmarkpath: {htmlFilePath}.");

            if (_tinyCitySettings.MarkdownFiles.Count > 0)
            {
                AnsiConsole.MarkupLine($" - Markdown Files:");
                foreach (var file in _tinyCitySettings.MarkdownFiles)
                {
                    AnsiConsole.MarkupLine($"   - {file}");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($" - Markdown Files: (none)");
            }
        }

        private void AddMarkdownFile(string markdownFile)
        {
            if (!_tinyCitySettings.MarkdownFiles.Contains(markdownFile))
            {
                _tinyCitySettings.MarkdownFiles.Add(markdownFile);
                TinyCitySettings.Save(_tinyCitySettings);
                AnsiConsole.MarkupLine($"[bold green]Added markdown file: {markdownFile}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold red]Markdown file '{markdownFile}' already exists.[/]");
            }
        }

        private void RemoveMarkdownFile(string markdownFile)
        {
            if (_tinyCitySettings.MarkdownFiles.Contains(markdownFile))
            {
                _tinyCitySettings.MarkdownFiles.Remove(markdownFile);
                TinyCitySettings.Save(_tinyCitySettings);
                AnsiConsole.MarkupLine($"[bold green]Removed markdown file: {markdownFile}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold yellow]Markdown file '{markdownFile}' wasn't found in the configuration.[/]");
            }
        }

        private void SetBrowser(string browser)
        {
            string browserLower = browser.ToLowerInvariant();
            switch (browserLower)
            {
                case "chrome":
                    _tinyCitySettings.BrowserPath = BrowserKnownPaths.ChromePath;
                    break;
                case "opera":
                    _tinyCitySettings.BrowserPath = BrowserKnownPaths.OperaPath;
                    break;
                case "brave":
                    _tinyCitySettings.BrowserPath = BrowserKnownPaths.BravePath;
                    break;
                case "edge":
                    _tinyCitySettings.BrowserPath = BrowserKnownPaths.EdgePath;
                    break;
                default:
                    AnsiConsole.MarkupLine($"[bold red]Invalid browser type '{browserLower}'. Valid values are: chrome, opera, brave, edge.[/]");
                    return;
            }

            AnsiConsole.MarkupLine($"[bold green]Set browser path to {_tinyCitySettings.BrowserPath}[/]");
            TinyCitySettings.Save(_tinyCitySettings);
        }

        private void SetHtmlBookmarkFile(string htmlBookmarkFile)
        {
            _tinyCitySettings.HtmlBookmarksFile = htmlBookmarkFile;
            TinyCitySettings.Save(_tinyCitySettings);
            AnsiConsole.MarkupLine($"[bold green]Added HTML bookmark file: {htmlBookmarkFile}[/]");
        }
    }
}
