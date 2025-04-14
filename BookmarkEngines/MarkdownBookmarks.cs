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

        public MarkdownBookmarks()
        {
            string home = Environment.GetEnvironmentVariable("HOME");

            if (string.IsNullOrEmpty(home))
            {
                home = Environment.GetEnvironmentVariable("USERPROFILE");
            }

            string markdownPath = Path.Combine(home,"bookmarks.md");
            if (!Path.Exists(markdownPath))
            {
                AnsiConsole.MarkupLine($"[bold yellow] - Couldn't find '{markdownPath}' so skipping.[/]");
                return;
            }

            string markdown = File.ReadAllText(markdownPath);
            var document = Markdown.Parse(markdown);
            //AnsiConsole.MarkupLine($" - Loaded {markdownPath}.");

            Bookmarks = new List<BookmarkNode>();
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

                        Bookmarks.Add(bookmark);
                    }
                }
            }
        }
    }
}
