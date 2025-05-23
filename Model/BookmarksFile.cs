using System.Text.Json.Serialization;

namespace TinyCity.Model
{
    public class BookmarksFile
    {
        [JsonPropertyName("checksum")]
        public required string Checksum { get; set; }

        [JsonPropertyName("roots")]
        public required Roots Roots { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }
    }
}