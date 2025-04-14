# tiny-city
A console app written in C# that searches your bookmarks file (Chrome and ``~/bookmarks.md`` for now), and displays them in the terminal, 
optionally launching them in your default browser.

The terminal output includes links that can be clicked, using the Spectre console library.

## Usage

Requires [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later.

```
USAGE:
    tinycity [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    search <query>    Search the bookmarks
    list              List all bookmarks
```

```
dotnet run --
dotnet run -- ls
dotnet run -- search "google" -urls
dotnet run -- search "gmail"
dotnet run -- search "openrouter" -- launch
```