using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class PlaylistPage : IEquatable<PlaylistPage?>
    {
        [JsonPropertyName("maps")]
        public List<MapDetailWithOrder> Maps { get; set; } = new();

        [JsonPropertyName("playlist")]
        public Playlist? Playlist { get; set; } = new();

        public override bool Equals(object? obj)
        {
            return obj is PlaylistPage && this.Equals(obj as PlaylistPage);
        }

        public bool Equals(PlaylistPage? other)
        {
            return other is not null &&
                   this.Maps.SequenceEqual(other.Maps) &&
                   this.Playlist == other.Playlist;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Maps, this.Playlist);
        }

        public static bool operator ==(PlaylistPage? left, PlaylistPage? right)
        {
            return EqualityComparer<PlaylistPage>.Default.Equals(left, right);
        }

        public static bool operator !=(PlaylistPage? left, PlaylistPage? right)
        {
            return !(left == right);
        }
    }
}
