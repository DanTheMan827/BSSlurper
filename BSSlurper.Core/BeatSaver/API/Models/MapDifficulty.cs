using System.Text.Json.Serialization;

namespace BSSlurper.Core.BeatSaver.API.Models
{
    public partial class MapDifficulty : IEquatable<MapDifficulty?>
    {
        [JsonPropertyName("njs")]
        public double Njs { get; set; } = 0;

        [JsonPropertyName("offset")]
        public double Offset { get; set; } = 0;

        [JsonPropertyName("notes")]
        public long Notes { get; set; } = 0;

        [JsonPropertyName("bombs")]
        public long Bombs { get; set; } = 0;

        [JsonPropertyName("obstacles")]
        public long Obstacles { get; set; } = 0;

        [JsonPropertyName("nps")]
        public double Nps { get; set; } = 0;

        [JsonPropertyName("length")]
        public double Length { get; set; } = 0;

        [JsonPropertyName("characteristic")]
        public string? Characteristic { get; set; }

        [JsonPropertyName("difficulty")]
        public string? Difficulty { get; set; }

        [JsonPropertyName("events")]
        public long Events { get; set; } = 0;

        [JsonPropertyName("chroma")]
        public bool Chroma { get; set; }

        [JsonPropertyName("me")]
        public bool Me { get; set; }

        [JsonPropertyName("ne")]
        public bool Ne { get; set; }

        [JsonPropertyName("cinema")]
        public bool Cinema { get; set; }

        [JsonPropertyName("seconds")]
        public double Seconds { get; set; } = 0;

        [JsonPropertyName("paritySummary")]
        public MapParitySummary? ParitySummary { get; set; } = new();

        [JsonPropertyName("maxScore")]
        public long MaxScore { get; set; } = 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is MapDifficulty && this.Equals(obj as MapDifficulty);
        }

        public bool Equals(MapDifficulty? other)
        {
            return other is not null &&
                   this.Njs == other.Njs &&
                   this.Offset == other.Offset &&
                   this.Notes == other.Notes &&
                   this.Bombs == other.Bombs &&
                   this.Obstacles == other.Obstacles &&
                   this.Nps == other.Nps &&
                   this.Length == other.Length &&
                   this.Characteristic == other.Characteristic &&
                   this.Difficulty == other.Difficulty &&
                   this.Events == other.Events &&
                   this.Chroma == other.Chroma &&
                   this.Me == other.Me &&
                   this.Ne == other.Ne &&
                   this.Cinema == other.Cinema &&
                   this.Seconds == other.Seconds &&
                   this.ParitySummary == other.ParitySummary &&
                   this.MaxScore == other.MaxScore &&
                   this.Label == other.Label;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Njs);
            hash.Add(this.Offset);
            hash.Add(this.Notes);
            hash.Add(this.Bombs);
            hash.Add(this.Obstacles);
            hash.Add(this.Nps);
            hash.Add(this.Length);
            hash.Add(this.Characteristic);
            hash.Add(this.Difficulty);
            hash.Add(this.Events);
            hash.Add(this.Chroma);
            hash.Add(this.Me);
            hash.Add(this.Ne);
            hash.Add(this.Cinema);
            hash.Add(this.Seconds);
            hash.Add(this.ParitySummary);
            hash.Add(this.MaxScore);
            hash.Add(this.Label);
            return hash.ToHashCode();
        }

        public static bool operator ==(MapDifficulty? left, MapDifficulty? right)
        {
            return EqualityComparer<MapDifficulty>.Default.Equals(left, right);
        }

        public static bool operator !=(MapDifficulty? left, MapDifficulty? right)
        {
            return !(left == right);
        }
    }
}
