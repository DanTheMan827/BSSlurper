using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public class ResponseData<T> : IEquatable<ResponseData<T>?> where T : IUpdatedAt
    {
        [JsonPropertyName("docs")]
        public List<T> Docs { get; set; } = new();

        public override bool Equals(object? obj)
        {
            return obj is ResponseData<T> && this.Equals(obj as ResponseData<T>);
        }

        public bool Equals(ResponseData<T>? other)
        {
            return other is not null &&
                   this.Docs.SequenceEqual(other.Docs);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Docs);
        }

        public static bool operator ==(ResponseData<T>? left, ResponseData<T>? right)
        {
            return EqualityComparer<ResponseData<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(ResponseData<T>? left, ResponseData<T>? right)
        {
            return !(left == right);
        }
    }
}
