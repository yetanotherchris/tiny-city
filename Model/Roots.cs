using System.Text.Json.Serialization;

public class Roots
{
    [JsonPropertyName("bookmark_bar")]
    public required BookmarkNode BookmarkBar { get; set; }

    [JsonPropertyName("other")]
    public required BookmarkNode Other { get; set; }

    [JsonPropertyName("synced")]
    public required BookmarkNode Synced { get; set; }
}
