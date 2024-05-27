using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class MapDetailWithOrder : IEquatable<MapDetailWithOrder?>
    {
        [JsonPropertyName("map")]
        public MapDetail? Map { get; set; } = new();

        [JsonPropertyName("order")]
        public double Order { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            return obj is MapDetailWithOrder && this.Equals(obj as MapDetailWithOrder);
        }

        public bool Equals(MapDetailWithOrder? other)
        {
            return other is not null &&
                   this.Map == other.Map &&
                   this.Order == other.Order;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Map, this.Order);
        }
    }
}
