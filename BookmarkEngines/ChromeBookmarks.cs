﻿using System.Text.Json;
using TinyCity.Model;

namespace TinyCity.BookmarkEngines
{
    public class ChromeBookmarks
    {
        public List<BookmarkNode> FlattenedBookmarks { get; set; } = new List<BookmarkNode>();
        private string _log = "";

        public ChromeBookmarks(TinyCitySettings settings)
        {
            EnsureBookmarksPath(settings);
            if (string.IsNullOrEmpty(settings.BrowserBookmarkFullPath))
            {
                return;
            }

            string json = File.ReadAllText(settings.BrowserBookmarkFullPath);           
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

            _log = $" ✅ Browser bookmarks: Loaded {FlattenedBookmarks.Count} bookmarks from '{settings.BrowserBookmarkFullPath}'.";
        }

        public string GetLog()
        {
            return _log;
        }

        private void EnsureBookmarksPath(TinyCitySettings settings)
        {
            if (!string.IsNullOrEmpty(settings.BrowserBookmarkFullPath))
            {
                _log = $" ✅ Browser bookmarks: Using '{settings.BrowserBookmarkFullPath}'.";
                return;
            }

            string[] directoriesToTry =
            {
                Path.Combine(settings.BrowserPath, "Default", "Bookmarks"),
                Path.Combine(settings.BrowserPath, "Profile 1", "Bookmarks"),
                Path.Combine(settings.BrowserPath, "Profile 2", "Bookmarks")
            };

            foreach (string path in directoriesToTry)
            {
                if (Path.Exists(path))
                {
                    _log += $"• Browser bookmarks: Found '{path}'.\n";
                    settings.BrowserBookmarkFullPath = path;
                    TinyCitySettings.Save(settings);
                    return;
                }

                _log += $" ⚠️ Browser bookmarks: Couldn't find '{path}'.\n";
            }

            _log += $" ⚠️ Browser bookmarks: couldn't find a Browser bookmarks path, skipping.";
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
