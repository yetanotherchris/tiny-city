
using System.Diagnostics;
using System.Text.Json;
using Spectre.Console;

public class Program
{
    // https://spectreconsole.net/cli/introduction
    static void Main(string[] args)
    {
        string chromePath = Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"/Google/Chrome/User Data/Default/"; // Windows
        string bookmarksPath = $"{chromePath}/Bookmarks.bak";
        string json = File.ReadAllText(bookmarksPath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var bookmarks = JsonSerializer.Deserialize<BookmarksFile>(json, options);

        var flattenedBookmarks = new List<BookmarkNode>();
        if (bookmarks != null)
        {
            var bookmarkBarNodes = FlattenNodes(bookmarks.Roots.BookmarkBar);
            var otherNodes = FlattenNodes(bookmarks.Roots.Other);
            var syncedNodes = FlattenNodes(bookmarks.Roots.Synced);

            flattenedBookmarks.AddRange(bookmarkBarNodes);
            flattenedBookmarks.AddRange(otherNodes);
            flattenedBookmarks.AddRange(syncedNodes);
        }

        AnsiConsole.MarkupLine($"[bold green]{flattenedBookmarks.Count} Chrome bookmarks[/]");
        if (args.Length > 0)
        {
            if (args[0] == "all")
            {
                foreach (var bookmark in flattenedBookmarks.OrderBy(x => x.Name))
                {
                    string link = $"[link={bookmark.Url}]{bookmark.Name}[/]";
                    string urlHost = new Uri(bookmark.Url).Host;
                    AnsiConsole.MarkupLine($"[bold chartreuse1]{link}[/] ({urlHost})");
                }
            }
            else if (args[0].StartsWith("launch"))
            {
                string searchTerm = args[0].ToLower().Substring("launch".Length).Trim();
                var filteredBookmarks = flattenedBookmarks.Where(b => b.Name.ToLower().Contains(searchTerm)).ToList();
                foreach (var bookmark in flattenedBookmarks.OrderBy(x => x.Name))
                {
                    string link = $"[link={bookmark.Url}]{bookmark.Name}[/]";
                    string urlHost = new Uri(bookmark.Url).Host;
                    AnsiConsole.MarkupLine($"[bold chartreuse1]{link}[/] ({urlHost})");
                }

                var first = filteredBookmarks.FirstOrDefault();
                AnsiConsole.MarkupLine($"[bold green]Launching {first.Name}[/]");
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = first.Url,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            else
            {
                string searchTerm = args[0].ToLower();
                var filteredBookmarks = flattenedBookmarks.Where(b => b.Name.ToLower().Contains(searchTerm)).ToList();

                foreach (var bookmark in filteredBookmarks)
                {
                    Console.WriteLine($"{bookmark.Name} - {bookmark.Url}");
                }
            }
        }
    }

    static List<BookmarkNode> FlattenNodes(BookmarkNode bookmarkNode)
    {
        var bookmarksList = new List<BookmarkNode>();
        foreach (var bookmark in bookmarkNode.Children)
        {
            if (bookmark.Children?.Count > 0)
            {
                foreach (var child in bookmark.Children)
                {
                    if (child.Type == "url")
                    {
                        bookmarksList.Add(child);
                    }
                }
            }
            else
            {
                bookmarksList.Add(bookmark);
            }
        }

        return bookmarksList;
    }
}