using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Spectre.Console;
using TinyCity.Model;

namespace TinyCity.BookmarkEngines
{
    public class MarkdownBookmarks
    {
        public List<BookmarkNode> Bookmarks { get; set; } = new List<BookmarkNode>();

        public MarkdownBookmarks(TinyCitySettings settings)
        {
            Bookmarks = new List<BookmarkNode>();

            foreach (var file in settings.MarkdownFiles)
            {
                if (File.Exists(file))
                {
                    var bookmarks = ParseMarkdownFile(file);
                    Bookmarks.AddRange(bookmarks);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[bold yellow] - Couldn't find '{file}' so skipping.[/]");
                }
            }
        }

        private List<BookmarkNode> ParseMarkdownFile(string markdown)
        {
            var document = Markdown.Parse(markdown);

            var bookmarks = new List<BookmarkNode>();
            foreach (var inline in document.Descendants<LinkInline>())
            {
                if (inline is LinkInline linkInline)
                {
                    string linkText = linkInline.FirstChild?.ToString() ?? string.Empty;
                    string url = linkInline.Url;

                    if (!string.IsNullOrEmpty(linkText) && !string.IsNullOrEmpty(url))
                    {
                        var bookmark = new BookmarkNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = linkText,
                            Url = url,
                            Type = "url"
                        };

                        bookmarks.Add(bookmark);
                    }
                }
            }

            return bookmarks;
        }
    }
}
