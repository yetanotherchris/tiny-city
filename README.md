# tiny-city
A console app written in C# that searches your bookmarks file (Chrome for now), and displays them in the terminal, optionally launching them in your default browser.

The terminal output includes links that can be clicked, using the Spectre console library.

## Usage
*(command line argument parsing coming soon)*

```
# Show all bookmarks
dotnet run -- all

# Search a bookmark
dotnet run -- <my search term>

# Launch the first bookmark found
dotnet run -- launch <my search term>
```