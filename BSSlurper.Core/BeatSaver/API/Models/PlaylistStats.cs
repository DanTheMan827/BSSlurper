using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class PlaylistStats : IEquatable<PlaylistStats?>
    {
        [JsonPropertyName("totalMaps")]
        public long TotalMaps { get; set; } = 0;

        [JsonPropertyName("mapperCount")]
        public long MapperCount { get; set; } = 0;

        [JsonPropertyName("totalDuration")]
        public long TotalDuration { get; set; } = 0;

        [JsonPropertyName("minNps")]
        public double MinNps { get; set; } = 0;

        [JsonPropertyName("maxNps")]
        public double MaxNps { get; set; } = 0;

        [JsonPropertyName("upVotes")]
        public long UpVotes { get; set; } = 0;

        [JsonPropertyName("downVotes")]
        public long DownVotes { get; set; } = 0;

        [JsonPropertyName("avgScore")]
        public double AvgScore { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            return obj is PlaylistStats && this.Equals(obj as PlaylistStats);
        }

        public bool Equals(PlaylistStats? other)
        {
            return other is not null &&
                   this.TotalMaps == other.TotalMaps &&
                   this.MapperCount == other.MapperCount &&
                   this.TotalDuration == other.TotalDuration &&
                   this.MinNps == other.MinNps &&
                   this.MaxNps == other.MaxNps &&
                   this.UpVotes == other.UpVotes &&
                   this.DownVotes == other.DownVotes &&
                   this.AvgScore == other.AvgScore;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.TotalMaps, this.MapperCount, this.TotalDuration, this.MinNps, this.MaxNps, this.UpVotes, this.DownVotes, this.AvgScore);
        }

        public static bool operator ==(PlaylistStats? left, PlaylistStats? right)
        {
            return EqualityComparer<PlaylistStats>.Default.Equals(left, right);
        }

        public static bool operator !=(PlaylistStats? left, PlaylistStats? right)
        {
            return !(left == right);
        }
    }
}
