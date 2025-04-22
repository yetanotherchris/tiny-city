using Spectre.Console;
using TinyCity.Model;

namespace TinyCity.BookmarkEngines
{
    public class BookmarkAggregator
    {
        private ChromeBookmarks _chromeBookmarks;
        private MarkdownBookmarks _markdownBookmarks;
        private HtmlBookmarks _htmlBookmarks;

        public List<BookmarkNode> AllBookmarks { get; }

        public BookmarkAggregator(ChromeBookmarks chromeBookmarks, MarkdownBookmarks markdownBookmarks, HtmlBookmarks htmlBookmarks)
        {
            _chromeBookmarks = chromeBookmarks;
            _markdownBookmarks = markdownBookmarks;
            _htmlBookmarks = htmlBookmarks;

            AllBookmarks = new List<BookmarkNode>();
            AllBookmarks = [.. chromeBookmarks.FlattenedBookmarks, .. markdownBookmarks.Bookmarks, .. htmlBookmarks.Bookmarks];
            AllBookmarks = AllBookmarks.Distinct().ToList();
        }

        public void WriteLoadedLog()
        {
            AnsiConsole.MarkupLine(_chromeBookmarks.GetLog());
            AnsiConsole.MarkupLine(_htmlBookmarks.GetLog());

            foreach (string item in _markdownBookmarks.GetLog())
            {
                AnsiConsole.MarkupLine(item);
            }
        }
    }
}
