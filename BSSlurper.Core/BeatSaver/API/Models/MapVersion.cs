using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class MapVersion : IEquatable<MapVersion?>
    {
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonPropertyName("sageScore")]
        public long? SageScore { get; set; } = 0;

        [JsonPropertyName("diffs")]
        public List<MapDifficulty> Diffs { get; set; } = new();

        [JsonPropertyName("downloadURL")]
        public Uri? DownloadUrl { get; set; }

        [JsonPropertyName("coverURL")]
        public Uri? CoverUrl { get; set; }

        [JsonPropertyName("previewURL")]
        public Uri? PreviewUrl { get; set; }

        internal async Task LoadRelated(DbContext db)
        {
            await db.Entry(this)
                .Collection(v => v.Diffs)
                .LoadAsync();
        }

        public override bool Equals(object? obj)
        {
            return obj is MapVersion && this.Equals(obj as MapVersion);
        }

        public bool Equals(MapVersion? other)
        {
            return other is not null &&
                   this.Hash == other.Hash &&
                   this.State == other.State &&
                   this.CreatedAt?.UtcDateTime == other.CreatedAt?.UtcDateTime &&
                   this.SageScore == other.SageScore &&
                   this.Diffs.SequenceEqual(other.Diffs) &&
                   this.DownloadUrl == other.DownloadUrl &&
                   this.CoverUrl == other.CoverUrl &&
                   this.PreviewUrl == other.PreviewUrl;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Hash, this.State, this.CreatedAt, this.SageScore, this.Diffs, this.DownloadUrl, this.CoverUrl, this.PreviewUrl);
        }

        public static bool operator ==(MapVersion? left, MapVersion? right)
        {
            return EqualityComparer<MapVersion>.Default.Equals(left, right);
        }

        public static bool operator !=(MapVersion? left, MapVersion? right)
        {
            return !(left == right);
        }
    }
}
