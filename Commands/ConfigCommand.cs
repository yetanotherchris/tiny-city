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
    }

    public class ConfigCommand : Command<ConfigCommandSettings>
    {
        private readonly TinyCitySettings _settings;
        public ConfigCommand(TinyCitySettings settings)
        {
            _settings = settings;
        }

        public override int Execute(CommandContext context, ConfigCommandSettings settings)
        {
            if (settings.Show)
            {
                AnsiConsole.MarkupLine($"[bold green]Home Directory: {_settings.HomeDirectory}[/]");
                AnsiConsole.MarkupLine($"[bold green]Markdown Files:[/]");
                foreach (var file in _settings.MarkdownFiles)
                {
                    AnsiConsole.MarkupLine($" - {file}");
                }
            }
            else if (!string.IsNullOrEmpty(settings.AddMarkdownFile))
            {
                if (!_settings.MarkdownFiles.Contains(settings.AddMarkdownFile))
                {
                    _settings.MarkdownFiles.Add(settings.AddMarkdownFile);
                    TinyCitySettings.Save(_settings);
                    AnsiConsole.MarkupLine($"[bold green]Added markdown file: {settings.AddMarkdownFile}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[bold red]Markdown file '{settings.AddMarkdownFile}' already exists.[/]");
                }
            }
            else if (!string.IsNullOrEmpty(settings.RemoveMarkdownFile))
            {
                if (_settings.MarkdownFiles.Contains(settings.RemoveMarkdownFile))
                {
                    _settings.MarkdownFiles.Remove(settings.RemoveMarkdownFile);
                    TinyCitySettings.Save(_settings);
                    AnsiConsole.MarkupLine($"[bold green]Removed markdown file: {settings.RemoveMarkdownFile}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[bold yellow]Markdown file '{settings.RemoveMarkdownFile}' wasn't found in the configuration.[/]");
                }
            }

            return 0;
        }
    }
}
