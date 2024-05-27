using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class MapStats : IEquatable<MapStats?>
    {
        [JsonPropertyName("plays")]
        public long Plays { get; set; } = 0;

        [JsonPropertyName("downloads")]
        public long Downloads { get; set; } = 0;

        [JsonPropertyName("upvotes")]
        public long Upvotes { get; set; } = 0;

        [JsonPropertyName("downvotes")]
        public long Downvotes { get; set; } = 0;

        [JsonPropertyName("score")]
        public double Score { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            return obj is MapStats && this.Equals(obj as MapStats);
        }

        public bool Equals(MapStats? other)
        {
            return other is not null &&
                   this.Plays == other.Plays &&
                   this.Downloads == other.Downloads &&
                   this.Upvotes == other.Upvotes &&
                   this.Downvotes == other.Downvotes &&
                   this.Score == other.Score;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Plays, this.Downloads, this.Upvotes, this.Downvotes, this.Score);
        }

        public static bool operator ==(MapStats? left, MapStats? right)
        {
            return EqualityComparer<MapStats>.Default.Equals(left, right);
        }

        public static bool operator !=(MapStats? left, MapStats? right)
        {
            return !(left == right);
        }
    }
}
