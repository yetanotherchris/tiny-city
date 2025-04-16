using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace TinyCity.Commands
{

    public class ConfigCommandSettings : CommandSettings
    {
        [CommandOption("-a|--add-markdown-file")]
        [Description("Adds a markdown file to scan for links.")]
        public string MarkdownFile { get; set; }

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
            else if (!string.IsNullOrEmpty(settings.MarkdownFile))
            {
                if (!_settings.MarkdownFiles.Contains(settings.MarkdownFile))
                {
                    _settings.MarkdownFiles.Add(settings.MarkdownFile);
                    TinyCitySettings.Save(_settings);
                    AnsiConsole.MarkupLine($"[bold green]Added markdown file: {settings.MarkdownFile}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[bold red]Markdown file '{settings.MarkdownFile}' already exists.[/]");
                }
            }

            return 0;
        }
    }
}
