using BSSlurper.Core.BeatSaver.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BSSlurper.Core
{
    public class PlaylistWithMaps : Playlist, IEquatable<PlaylistWithMaps?>
    {
        public List<MapDetailWithOrder> Maps { get; set; } = new();

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as PlaylistWithMaps);
        }

        public bool Equals(PlaylistWithMaps? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   this.PlaylistId == other.PlaylistId &&
                   this.Name == other.Name &&
                   this.Description == other.Description &&
                   this.PlaylistImage == other.PlaylistImage &&
                   this.PlaylistImage512 == other.PlaylistImage512 &&
                   this.Owner == other.Owner &&
                   this.Stats == other.Stats &&
                   this.CreatedAt == other.CreatedAt &&
                   this.UpdatedAt == other.UpdatedAt &&
                   this.SongsChangedAt == other.SongsChangedAt &&
                   this.DownloadUrl == other.DownloadUrl &&
                   this.Type == other.Type &&
                   this.Maps.SequenceEqual(other.Maps);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(base.GetHashCode());
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
            hash.Add(this.Maps);
            return hash.ToHashCode();
        }

        internal async Task LoadRelated(DbContext db)
        {
            await db.Entry(this)
                .Collection(p => p.Maps)
                .LoadAsync();

            foreach (var map in Maps)
            {
                await db.Entry(map)
                    .Reference(m => m.Map)
                    .LoadAsync();

                if (map.Map != null)
                {
                    await map.Map.LoadRelated(db);
                }

            }
        }

        public static bool operator ==(PlaylistWithMaps? left, PlaylistWithMaps? right)
        {
            return EqualityComparer<PlaylistWithMaps>.Default.Equals(left, right);
        }

        public static bool operator !=(PlaylistWithMaps? left, PlaylistWithMaps? right)
        {
            return !(left == right);
        }
    }
}
