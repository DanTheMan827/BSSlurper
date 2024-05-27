using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class UserDetail : IEquatable<UserDetail?>
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("avatar")]
        public Uri? Avatar { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("admin")]
        public bool Admin { get; set; }

        [JsonPropertyName("curator")]
        public bool Curator { get; set; }

        [JsonPropertyName("seniorCurator")]
        public bool SeniorCurator { get; set; }

        [JsonPropertyName("playlistUrl")]
        public Uri? PlaylistUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("curatorTab")]
        public bool? CuratorTab { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("verifiedMapper")]
        public bool? VerifiedMapper { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is UserDetail && this.Equals(obj as UserDetail);
        }

        public bool Equals(UserDetail? other)
        {
            return other is not null &&
                   this.Id == other.Id &&
                   this.Name == other.Name &&
                   this.Avatar == other.Avatar &&
                   this.Type == other.Type &&
                   this.Admin == other.Admin &&
                   this.Curator == other.Curator &&
                   this.SeniorCurator == other.SeniorCurator &&
                   this.PlaylistUrl == other.PlaylistUrl &&
                   this.Hash == other.Hash &&
                   this.CuratorTab == other.CuratorTab &&
                   this.VerifiedMapper == other.VerifiedMapper;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Id);
            hash.Add(this.Name);
            hash.Add(this.Avatar);
            hash.Add(this.Type);
            hash.Add(this.Admin);
            hash.Add(this.Curator);
            hash.Add(this.SeniorCurator);
            hash.Add(this.PlaylistUrl);
            hash.Add(this.Hash);
            hash.Add(this.CuratorTab);
            hash.Add(this.VerifiedMapper);
            return hash.ToHashCode();
        }

        public static bool operator ==(UserDetail? left, UserDetail? right)
        {
            return EqualityComparer<UserDetail>.Default.Equals(left, right);
        }

        public static bool operator !=(UserDetail? left, UserDetail? right)
        {
            return !(left == right);
        }
    }
}
