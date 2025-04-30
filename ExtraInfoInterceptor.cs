using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Cli;
using TinyCity.Commands;

namespace TinyCity
{
    public class ExtraInfoInterceptor : ICommandInterceptor
    {
        private bool _showExtraInfo;

        public ExtraInfoInterceptor()
        {
            _showExtraInfo = false;
        }

        public void Intercept(CommandContext context, CommandSettings settings)
        {
            if (settings is BaseSettings baseSettings && baseSettings.Extra)
            {
                _showExtraInfo = true;
            }
        }

        public void ShowOutput(Stopwatch stopwatch, Exception ex)
        {
            if (_showExtraInfo)
            {
                if (ex != null)
                {
                    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                }

                AnsiConsole.MarkupLine($"[italic skyblue1]Took {stopwatch.ElapsedMilliseconds}ms to complete.[/]");
            }
            else
            {
                if (ex != null)
                {
                    AnsiConsole.MarkupLine($"{ex.Message} Use --help for usage.");
                }
            }
        }
    }
}