using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class Playlist : IUpdatedAt, IEquatable<Playlist?>
    {
        [JsonPropertyName("playlistId")]
        public long PlaylistId { get; set; } = 0;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("playlistImage")]
        public Uri? PlaylistImage { get; set; }

        [JsonPropertyName("playlistImage512")]
        public Uri? PlaylistImage512 { get; set; }

        [JsonPropertyName("owner")]
        public UserDetail? Owner { get; set; } = new();

        [JsonPropertyName("stats")]
        public PlaylistStats? Stats { get; set; } = new();

        [JsonPropertyName("createdAt")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonPropertyName("songsChangedAt")]
        public DateTimeOffset? SongsChangedAt { get; set; }

        [JsonPropertyName("downloadURL")]
        public Uri? DownloadUrl { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Playlist && this.Equals(obj as Playlist);
        }

        public bool Equals(Playlist? other)
        {
            return other is not null &&
                   this.PlaylistId == other.PlaylistId &&
                   this.Name == other.Name &&
                   this.Description == other.Description &&
                   this.PlaylistImage == other.PlaylistImage &&
                   this.PlaylistImage512 == other.PlaylistImage512 &&
                   this.Owner == other.Owner &&
                   this.Stats == other.Stats &&
                   this.CreatedAt?.UtcDateTime == other.CreatedAt?.UtcDateTime &&
                   this.UpdatedAt?.UtcDateTime == other.UpdatedAt?.UtcDateTime &&
                   this.SongsChangedAt?.UtcDateTime == other.SongsChangedAt?.UtcDateTime &&
                   this.DownloadUrl == other.DownloadUrl &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.PlaylistId);
            hash.Add(this.Name);
            hash.Add(this.Description);
            hash.Add(this.PlaylistImage);
            hash.Add(this.PlaylistImage512);
            hash.Add(this.Owner);
            hash.Add(this.Stats);
            hash.Add(this.CreatedAt);
            hash.Add(this.UpdatedAt);
            hash.Add(this.SongsChangedAt);
            hash.Add(this.DownloadUrl);
            hash.Add(this.Type);
            return hash.ToHashCode();
        }

        public static bool operator ==(Playlist? left, Playlist? right)
        {
            return EqualityComparer<Playlist>.Default.Equals(left, right);
        }

        public static bool operator !=(Playlist? left, Playlist? right)
        {
            return !(left == right);
        }
    }
}
