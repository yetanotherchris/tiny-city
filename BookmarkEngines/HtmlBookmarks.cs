using Spectre.Console;
using TinyCity.Model;
using AngleSharp;
using AngleSharp.Dom;

namespace TinyCity.BookmarkEngines
{
    /// <summary>
    /// HTML bookmarks exports, using the Netscape format. The HTML Format is:
    /// <![CDATA[
    /// <DT><A HREF=..
    /// ]]>
    /// However, this parser doesn't care, it just grabs all the <a> tags and their href attributes.
    /// </summary>
    public class HtmlBookmarks
    {
        public List<BookmarkNode> Bookmarks { get; set; } = new List<BookmarkNode>();

        public HtmlBookmarks(TinyCitySettings settings)
        {
            string htmlFilePath = settings.HtmlBookmarksFile;
            if (string.IsNullOrEmpty(htmlFilePath))
            {
                AnsiConsole.MarkupLine($" - HTML bookmarks: no file specified in settings so skipping");
                return;
            }
            else if (!File.Exists(htmlFilePath))
            {
                AnsiConsole.MarkupLine($"[bold yellow] - HTML bookmarks: couldn't find '{htmlFilePath}' so skipping.[/]");
                return;
            }

            string html = File.ReadAllText(htmlFilePath);
            var bookmarks = ParseHtmlFile(html).GetAwaiter().GetResult();
            Bookmarks.AddRange(bookmarks);

            AnsiConsole.MarkupLine($" - HTML bookmarks: loaded {bookmarks.Count} bookmarks from '{htmlFilePath}'.");
        }

        private async Task<List<BookmarkNode>> ParseHtmlFile(string html)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(m => m.Content(html));

            var bookmarks = new List<BookmarkNode>();
            foreach (var anchor in document.QuerySelectorAll("a"))
            {
                string text = anchor.Text();
                string? href = anchor.Attributes["href"]?.Value;

                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(href))
                {
                    var bookmark = new BookmarkNode
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = text,
                        Url = href,
                        Type = "url"
                    };

                    bookmarks.Add(bookmark);
                }

            }

            return bookmarks;
        }
    }
}
