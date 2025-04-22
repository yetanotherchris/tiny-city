using System.Text.Json;
using TinyCity.Model;

namespace TinyCity.BookmarkEngines
{
    public class ChromeBookmarks
    {
        public List<BookmarkNode> FlattenedBookmarks { get; set; } = new List<BookmarkNode>();
        private string _log;

        public ChromeBookmarks(TinyCitySettings settings)
        {
            string bookmarksPath = Path.Combine(settings.BrowserPath, "Bookmarks");

            if (!Path.Exists(bookmarksPath))
            {
                _log = $" - Browser bookmarks: Couldn't find '{bookmarksPath}' so skipping.";
                return;
            }

            string json = File.ReadAllText(bookmarksPath);           
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var bookmarks = JsonSerializer.Deserialize<BookmarksFile>(json, options);

            FlattenedBookmarks = new List<BookmarkNode>();
            if (bookmarks != null)
            {
                var bookmarkBarNodes = FlattenNodes(bookmarks.Roots.BookmarkBar);
                var otherNodes = FlattenNodes(bookmarks.Roots.Other);
                var syncedNodes = FlattenNodes(bookmarks.Roots.Synced);

                FlattenedBookmarks = [.. bookmarkBarNodes, .. otherNodes, .. syncedNodes];
            }

            _log = $" - Browser bookmarks: Loaded {FlattenedBookmarks.Count} bookmarks from '{bookmarksPath}'.";
        }

        public string GetLog()
        {
            return _log;
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
