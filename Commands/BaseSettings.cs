using Spectre.Console.Cli;
using System.ComponentModel;

namespace TinyCity.Commands
{
    public class BaseSettings : CommandSettings
    {
        [CommandOption("--extra")]
        [Description("Displays extra information including how long the application took to run.")]
        public bool Extra { get; set; }
    }
}
