using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class MapDetail : IUpdatedAt, IEquatable<MapDetail?>
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("uploader")]
        public UserDetail? Uploader { get; set; } = new();

        [JsonPropertyName("metadata")]
        public MapDetailMetadata? Metadata { get; set; } = new();

        [JsonPropertyName("stats")]
        public MapStats? Stats { get; set; } = new();

        [JsonPropertyName("uploaded")]
        public DateTimeOffset? Uploaded { get; set; }

        [JsonPropertyName("automapper")]
        public bool Automapper { get; set; }

        [JsonPropertyName("ranked")]
        public bool Ranked { get; set; }

        [JsonPropertyName("qualified")]
        public bool Qualified { get; set; }

        [JsonPropertyName("versions")]
        public List<MapVersion> Versions { get; set; } = new();

        [JsonIgnore]
        public List<MapVersion> OldVersions { get; set; } = new();

        [JsonPropertyName("createdAt")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonPropertyName("lastPublishedAt")]
        public DateTimeOffset? LastPublishedAt { get; set; }

        [JsonPropertyName("bookmarked")]
        public bool Bookmarked { get; set; }

        [JsonPropertyName("declaredAi")]
        public string? DeclaredAi { get; set; }

        [JsonPropertyName("blRanked")]
        public bool BlRanked { get; set; }

        [JsonPropertyName("blQualified")]
        public bool BlQualified { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tags")]
        [JsonConverter(typeof(Tag.TagListConverter))]
        public List<Tag> Tags { get; set; } = new();

        public async Task LoadRelated(DbContext db)
        {
            await db.Entry(this)
                .Reference(m => m.Uploader)
                .LoadAsync();

            await db.Entry(this)
                .Collection(m => m.Versions)
                .LoadAsync();

            await db.Entry(this)
                .Collection(m => m.OldVersions)
                .LoadAsync();

            await db.Entry(this)
                .Collection(m => m.Tags)
                .LoadAsync();

            foreach (var version in Versions.Concat(OldVersions))
            {
                await db.Entry(version)
                    .Collection(v => v.Diffs)
                    .LoadAsync();
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is MapDetail && this.Equals(obj as MapDetail);
        }

        public bool Equals(MapDetail? other, bool dbTags = false)
        {
            return other is not null &&
                   this.Id == other.Id &&
                   this.Name == other.Name &&
                   this.Description == other.Description &&
                   this.Uploader == other.Uploader &&
                   this.Metadata == other.Metadata &&
                   this.Stats == other.Stats &&
                   this.Uploaded?.UtcDateTime == other.Uploaded?.UtcDateTime &&
                   this.Automapper == other.Automapper &&
                   this.Ranked == other.Ranked &&
                   this.Qualified == other.Qualified &&
                   this.Versions.SequenceEqual(other.Versions) &&
                   this.CreatedAt?.UtcDateTime == other.CreatedAt?.UtcDateTime &&
                   this.UpdatedAt?.UtcDateTime == other.UpdatedAt?.UtcDateTime &&
                   this.LastPublishedAt?.UtcDateTime == other.LastPublishedAt?.UtcDateTime &&
                   this.Bookmarked == other.Bookmarked &&
                   this.DeclaredAi == other.DeclaredAi &&
                   this.BlRanked == other.BlRanked &&
                   this.BlQualified == other.BlQualified &&
                   this.Tags.OrderBy(t => t.Name).SequenceEqual(other.Tags.OrderBy(t => t.Name));
        }

        public bool Equals(MapDetail? other) => Equals(other, false);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Id);
            hash.Add(this.Name);
            hash.Add(this.Description);
            hash.Add(this.Uploader);
            hash.Add(this.Metadata);
            hash.Add(this.Stats);
            hash.Add(this.Uploaded);
            hash.Add(this.Automapper);
            hash.Add(this.Ranked);
            hash.Add(this.Qualified);
            hash.Add(this.Versions);
            hash.Add(this.CreatedAt);
            hash.Add(this.UpdatedAt);
            hash.Add(this.LastPublishedAt);
            hash.Add(this.Bookmarked);
            hash.Add(this.DeclaredAi);
            hash.Add(this.BlRanked);
            hash.Add(this.BlQualified);
            hash.Add(this.Tags);
            return hash.ToHashCode();
        }

        public static bool operator ==(MapDetail? left, MapDetail? right)
        {
            return EqualityComparer<MapDetail>.Default.Equals(left, right);
        }

        public static bool operator !=(MapDetail? left, MapDetail? right)
        {
            return !(left == right);
        }
    }
}
