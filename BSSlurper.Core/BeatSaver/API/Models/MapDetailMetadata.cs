using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class MapDetailMetadata : IEquatable<MapDetailMetadata?>
    {
        [JsonPropertyName("bpm")]
        public double Bpm { get; set; } = 0;

        [JsonPropertyName("duration")]
        public long Duration { get; set; } = 0;

        [JsonPropertyName("songName")]
        public string? SongName { get; set; }

        [JsonPropertyName("songSubName")]
        public string? SongSubName { get; set; }

        [JsonPropertyName("songAuthorName")]
        public string? SongAuthorName { get; set; }

        [JsonPropertyName("levelAuthorName")]
        public string? LevelAuthorName { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is MapDetailMetadata && this.Equals(obj as MapDetailMetadata);
        }

        public bool Equals(MapDetailMetadata? other)
        {
            return other is not null &&
                   this.Bpm == other.Bpm &&
                   this.Duration == other.Duration &&
                   this.SongName == other.SongName &&
                   this.SongSubName == other.SongSubName &&
                   this.SongAuthorName == other.SongAuthorName &&
                   this.LevelAuthorName == other.LevelAuthorName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Bpm, this.Duration, this.SongName, this.SongSubName, this.SongAuthorName, this.LevelAuthorName);
        }

        public static bool operator ==(MapDetailMetadata? left, MapDetailMetadata? right)
        {
            return EqualityComparer<MapDetailMetadata>.Default.Equals(left, right);
        }

        public static bool operator !=(MapDetailMetadata? left, MapDetailMetadata? right)
        {
            return !(left == right);
        }
    }
}
