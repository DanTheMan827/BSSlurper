using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class MapParitySummary : IEquatable<MapParitySummary?>
    {
        [JsonPropertyName("errors")]
        public long Errors { get; set; } = 0;

        [JsonPropertyName("warns")]
        public long Warns { get; set; } = 0;

        [JsonPropertyName("resets")]
        public long Resets { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            return obj is MapParitySummary && this.Equals(obj as MapParitySummary);
        }

        public bool Equals(MapParitySummary? other)
        {
            return other is not null &&
                   this.Errors == other.Errors &&
                   this.Warns == other.Warns &&
                   this.Resets == other.Resets;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Errors, this.Warns, this.Resets);
        }

        public static bool operator ==(MapParitySummary? left, MapParitySummary? right)
        {
            return EqualityComparer<MapParitySummary>.Default.Equals(left, right);
        }

        public static bool operator !=(MapParitySummary? left, MapParitySummary? right)
        {
            return !(left == right);
        }
    }
}
