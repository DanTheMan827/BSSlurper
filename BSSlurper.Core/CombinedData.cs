using BSSlurper.Core.BeatSaver.API.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BSSlurper.Core
{
    public class CombinedData
    {
        [JsonPropertyName("maps")]
        public List<MapDetail> Maps { get; set; } = new List<MapDetail>();

        [JsonPropertyName("playlists")]
        public List<PlaylistPage> Playlists { get; set; } = new List<PlaylistPage>();

        public static async Task<CombinedData?> Deserialize(string filename, bool nullOnError = false, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var file = File.OpenRead(filename))
                {
                    var data = await JsonSerializer.DeserializeAsync<CombinedData>(file, JsonSerializerOptions.Default, cancellationToken);
                    return data;
                }
            }
            catch (Exception ex)
            {
                if (nullOnError)
                {
                    return null;
                }

                throw;
            }
        }
    }
}
