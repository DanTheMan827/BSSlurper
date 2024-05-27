
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BSSlurper.Core
{
    public class Tag : IEquatable<Tag?>
    {
        public string? Name { get; set; }

        public Tag()
        {
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as Tag);
        }

        public bool Equals(Tag? other)
        {
            return other is not null &&
                   this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name);
        }

        public static bool operator ==(Tag? left, Tag? right)
        {
            return EqualityComparer<Tag>.Default.Equals(left, right);
        }

        public static bool operator !=(Tag? left, Tag? right)
        {
            return !(left == right);
        }

        public class TagConverter : JsonConverter<Tag>
        {
            public override Tag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    return new Tag()
                    {
                        Name = reader.GetString()
                    };
                }
                throw new JsonException("Expected a string value.");
            }

            public override void Write(Utf8JsonWriter writer, Tag value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Name);
            }
        }

        public class TagListConverter : JsonConverter<List<Tag>>
        {
            public override List<Tag> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    var tags = new List<Tag>();

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                            break;

                        if (reader.TokenType == JsonTokenType.String)
                        {
                            tags.Add(new Tag()
                            {
                                Name = reader.GetString()
                            });
                        }
                        else
                        {
                            throw new JsonException("Expected a string value.");
                        }
                    }

                    return tags.OrderBy(t => t.Name).ToList();
                }
                throw new JsonException("Expected an array of strings.");
            }

            public override void Write(Utf8JsonWriter writer, List<Tag> value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                foreach (var tag in value)
                {
                    writer.WriteStringValue(tag.Name);
                }
                writer.WriteEndArray();
            }
        }
    }
}
