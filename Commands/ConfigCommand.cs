using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Linq;

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
    }

    public class ConfigCommand : Command<ConfigCommandSettings>
    {
        private readonly TinyCitySettings _tinyCitySettings;
        public ConfigCommand(TinyCitySettings settings)
        {
            _tinyCitySettings = settings;
        }

        public override int Execute(CommandContext context, ConfigCommandSettings settings)
        {
            if (settings.Show)
            {
                AnsiConsole.MarkupLine($"[bold green]Loaded config file from: {TinyCitySettings.GetConfigFilePath()}[/]");
                AnsiConsole.MarkupLine($"[green]- Home Directory: {_tinyCitySettings.HomeDirectory}[/]");
                AnsiConsole.MarkupLine($"[green]- Browser path: {_tinyCitySettings.BrowserPath}[/]");
                AnsiConsole.MarkupLine($"[green]- Markdown Files:[/]");
                foreach (var file in _tinyCitySettings.MarkdownFiles)
                {
                    AnsiConsole.MarkupLine($" - {file}");
                }
            }
            else if (!string.IsNullOrEmpty(settings.AddMarkdownFile))
            {
                if (!_tinyCitySettings.MarkdownFiles.Contains(settings.AddMarkdownFile))
                {
                    _tinyCitySettings.MarkdownFiles.Add(settings.AddMarkdownFile);
                    TinyCitySettings.Save(_tinyCitySettings);
                    AnsiConsole.MarkupLine($"[bold green]Added markdown file: {settings.AddMarkdownFile}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[bold red]Markdown file '{settings.AddMarkdownFile}' already exists.[/]");
                }
            }
            else if (!string.IsNullOrEmpty(settings.RemoveMarkdownFile))
            {
                if (_tinyCitySettings.MarkdownFiles.Contains(settings.RemoveMarkdownFile))
                {
                    _tinyCitySettings.MarkdownFiles.Remove(settings.RemoveMarkdownFile);
                    TinyCitySettings.Save(_tinyCitySettings);
                    AnsiConsole.MarkupLine($"[bold green]Removed markdown file: {settings.RemoveMarkdownFile}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[bold yellow]Markdown file '{settings.RemoveMarkdownFile}' wasn't found in the configuration.[/]");
                }
            }
            else if (!string.IsNullOrEmpty(settings.Browser))
            {
                string browser = settings.Browser.ToLowerInvariant();
                switch (browser)
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
                        AnsiConsole.MarkupLine($"[bold red]Invalid browser type '{browser}'. Valid values are: chrome, opera, brave, edge.[/]");
                        return 1;
                }

                AnsiConsole.MarkupLine($"[bold green]Set browser path to {_tinyCitySettings.BrowserPath }[/]");
                TinyCitySettings.Save(_tinyCitySettings);
            }

            return 0;
        }
    }
}
