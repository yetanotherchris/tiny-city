using System.Text.Json.Serialization;

public class BookmarkNode
{
    [JsonPropertyName("date_added")]
    public string? DateAdded { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("date_modified")]
    public string? DateModified { get; set; }

    [JsonPropertyName("children")]
    public List<BookmarkNode>? Children { get; set; }
}
