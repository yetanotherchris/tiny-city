using Spectre.Console;
using System.Text.Json;
using TinyCity.Model;

namespace TinyCity.BookmarkEngines
{
    public class ChromeBookmarks
    {
        public List<BookmarkNode> FlattenedBookmarks { get; set; } = new List<BookmarkNode>();

        public ChromeBookmarks()
        {
            string chromePath = "";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                chromePath = Path.Combine(homePath, ".config","google-chrome","Default");
            }
            else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // todo
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                string localAppDataPath = Environment.GetEnvironmentVariable("LOCALAPPDATA");
                chromePath = Path.Combine(localAppDataPath, "Google","Chrome","User Data", "Default"); // Windows
            }

            string bookmarksPath = Path.Combine(chromePath, "Bookmarks");
            if (!Path.Exists(bookmarksPath))
            {
                AnsiConsole.MarkupLine($"[bold yellow] - Couldn't find '{bookmarksPath}' so skipping.[/]");
                return;
            }

            string json = File.ReadAllText(bookmarksPath);           
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var bookmarks = JsonSerializer.Deserialize<BookmarksFile>(json, options);
            //AnsiConsole.MarkupLine($" - Loaded {bookmarksPath}.");

            FlattenedBookmarks = new List<BookmarkNode>();
            if (bookmarks != null)
            {
                var bookmarkBarNodes = FlattenNodes(bookmarks.Roots.BookmarkBar);
                var otherNodes = FlattenNodes(bookmarks.Roots.Other);
                var syncedNodes = FlattenNodes(bookmarks.Roots.Synced);

                FlattenedBookmarks.AddRange(bookmarkBarNodes);
                FlattenedBookmarks.AddRange(otherNodes);
                FlattenedBookmarks.AddRange(syncedNodes);
            }
        }

        static List<BookmarkNode> FlattenNodes(BookmarkNode bookmarkNode)
        {
            if (bookmarkNode.Children == null)
                return new List<BookmarkNode>();

            var bookmarksList = new List<BookmarkNode>();
            foreach (BookmarkNode bookmark in bookmarkNode.Children)
            {
                Recurse(bookmarksList, bookmark);
            }

            return bookmarksList;
        }

        static void Recurse(List<BookmarkNode> list, BookmarkNode bookmark)
        {
            if (bookmark.Children?.Count > 0)
            {
                foreach (var child in bookmark.Children)
                {
                    if (child.Type == "url")
                    {
                        list.Add(child);
                    }

                    if (child.Children?.Count > 0)
                    {
                        Recurse(list, child);
                    }
                }
            }
            else
            {
                list.Add(bookmark);
            }
        }
    }
}
