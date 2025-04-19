# tiny-city
A console app written in C# that searches your bookmarks file (Chrome and ``~/bookmarks.md`` for now), and displays them in the terminal, 
optionally launching them in your default browser.

The terminal output includes links that can be clicked, using the Spectre console library.

## Download

[![GitHub Release](https://img.shields.io/github/v/release/yetanotherchris/tiny-city?logo=github&sort=semver)](https://github.com/yetanotherchris/tiny-city/releases/latest/)

You can download the latest version of tinycity using PowerShell or Bash:

```powershell
Invoke-WebRequest -Uri "https://github.com/yetanotherchris/tiny-city/releases/latest/download/tinycity.exe" -OutFile "tinycity.exe"
```
```bash
wget -O tinycity "https://github.com/yetanotherchris/tiny-city/releases/latest/download/tinycity"
```
```bash
curl -o tinycity -L "https://github.com/yetanotherchris/tiny-city/releases/latest/download/tinycity"
```

You can also download the latest release directly from the [Releases page](https://github.com/yetanotherchris/tiny-city/releases).

## Usage

```
USAGE:
    tinycity [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    search <query>    Search the bookmarks
    list              List all bookmarks
    config            Configure bookmark sources
```

### Examples
```
./tinycity ls
./tinycity search "google" -urls
./tinycity search "gmail"
./tinycity search "openrouter" --launch
./tinycity config -b brave
./tinycity config -a more-bookmarks.md
```

If you clone the source using `git clone` (requires [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later):

```
dotnet run --
dotnet run -- ls
dotnet run -- search "google" -urls
dotnet run -- search "gmail"
dotnet run -- search "openrouter" -- launch
```

### Why the tinycity name?
The name was generated using a name generator.