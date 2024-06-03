using BSSlurper.Core.BeatSaver.API.Models;
using System.Globalization;
using System.Text.Json;

namespace BSSlurper.Core.BeatSaver.API
{
    public static class ApiClient
    {
        public const string PlaylistsEndpoint = "https://api.beatsaver.com/playlists/latest";
        public const string MapsEndpoint = "https://api.beatsaver.com/maps/latest";
        public const string ApiDateFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
        public delegate Task DataCallback<T>(T data, bool pageEnd, CancellationToken cancellationToken = default);

        internal static async Task<T> GetJsonAsync<T>(string url, int retries = 3, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (int i = 0; i < retries; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var client = new HttpClient();
                    Console.WriteLine($"Fetching: {url}");
                    var response = await client.GetAsync(url, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);
                    return await JsonSerializer.DeserializeAsync<T>(responseBody, JsonSerializerOptions.Default, cancellationToken);
                }
                catch (Exception)
                {
                    if (i == retries - 1)
                    {
                        throw;
                    }
                }
            }

            throw new Exception("Exceeded retry attempts.");
        }

        /// <summary>
        /// Fetches data from an API endpoint with pagination and date filters.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="perPage">Number of items per page (1-100).</param>
        /// <param name="before">Fetch data before this date (YYYY-MM-DDTHH:MM:SS+00:00).</param>
        /// <param name="after">Fetch data after this date (YYYY-MM-DDTHH:MM:SS+00:00).</param>
        /// <returns>A JSON-parsed object.</returns>
        private static async Task<ResponseData<T>> GetDatedDataAsync<T>(string endpoint, int perPage = 100, string? before = null, string? after = null, CancellationToken cancellationToken = default) where T : IUpdatedAt
        {
            var queryParams = new Dictionary<string, string>
        {
            { "pageSize", perPage.ToString() },
            { "sort", "UPDATED" }
        };

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(before))
            {
                queryParams.Add("before", before);
            }

            if (!string.IsNullOrEmpty(after))
            {
                queryParams.Add("after", after);
            }

            if (endpoint == MapsEndpoint)
            {
                queryParams.Add("automapper", "true");
            }

            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync(cancellationToken).Result;
            var url = $"{endpoint}?{queryString}";

            return await GetJsonAsync<ResponseData<T>>(url, 3, cancellationToken);
        }

        /// <summary>
        /// Fetches all data from an API endpoint after a specified date.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="after">Fetch data more recent than this date (YYYY-MM-DDTHH:MM:SS+00:00).</param>
        /// <returns>A list of items.</returns>
        private static async Task<List<T>> GetAllDatedDataAfterAsync<T>(string endpoint, string? after = null, CancellationToken cancellationToken = default) where T : IUpdatedAt
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<T>();

            await GetAllDatedDataAfterAsync<T>(endpoint, async (data, pageEnd, cancellationToken) => await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                items.Add(data);
            }), after, cancellationToken);

            return items;
        }

        /// <summary>
        /// Fetches all data from an API endpoint after a specified date.
        /// </summary>
        /// <param name="callback">A callback called after each item is fetched.</param>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="after">Fetch data more recent than this date (YYYY-MM-DDTHH:MM:SS+00:00).</param>
        /// <returns></returns>
        private static async Task GetAllDatedDataAfterAsync<T>(string endpoint, DataCallback<T> callback, string? after = null, CancellationToken cancellationToken = default) where T : IUpdatedAt
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTimeOffset? lastDate = null;
            DateTimeOffset? parsedAfter = after != null ? DateTime.Parse(after) : null;

            while (!cancellationToken.IsCancellationRequested)
            {
                var data = await GetDatedDataAsync<T>(endpoint, 100, lastDate?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), after, cancellationToken);

                if (data.Docs.Count == 0)
                {
                    break;
                }

                var items = data.Docs.Select((doc, index) => (doc, index)).ToArray();

                foreach (var item in items)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (after != null && item.doc.UpdatedAt <= parsedAfter)
                    {
                        break;
                    }

                    lastDate = item.doc.UpdatedAt;

                    await callback(item.doc, item.index == items.Length - 1);
                }
            }
        }

        /// <summary>
        /// Fetches all data from an API endpoint before a specified date.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="after">Fetch data more recent than this date (YYYY-MM-DDTHH:MM:SS+00:00).</param>
        /// <returns>A list of items.</returns>
        private static async Task<List<T>> GetAllDatedDataBeforeAsync<T>(string endpoint, string? before = null, CancellationToken cancellationToken = default) where T : IUpdatedAt
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<T>();

            await GetAllDatedDataBeforeAsync<T>(endpoint, async (data, pageEnd, cancellationToken) => await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                items.Add(data);
            }), before, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            return items;
        }

        /// <summary>
        /// Fetches all data from an API endpoint before a specified date.
        /// </summary>
        /// <param name="callback">A callback called after each item is fetched.</param>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="after">Fetch data more recent than this date (YYYY-MM-DDTHH:MM:SS+00:00).</param>
        /// <returns></returns>
        private static async Task GetAllDatedDataBeforeAsync<T>(string endpoint, DataCallback<T> callback, string? before = null, CancellationToken cancellationToken = default) where T : IUpdatedAt
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTimeOffset? lastDate = before != null ? DateTime.Parse(before) : null;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = await GetDatedDataAsync<T>(endpoint, 100, lastDate?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), null, cancellationToken);

                if (data.Docs.Count == 0)
                {
                    break;
                }

                foreach (var item in data.Docs.Select((doc, index) => (doc, index)))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    lastDate = item.doc.UpdatedAt;

                    await callback(item.doc, item.index == data.Docs.Count - 1);
                }
            }
        }

        public static async Task<List<Playlist>> GetAllPlaylistsAfterAsync(string? after = null, CancellationToken cancellationToken = default) => await GetAllDatedDataAfterAsync<Playlist>(PlaylistsEndpoint, after, cancellationToken);
        public static async Task<List<MapDetail>> GetAllMapsAfterAsync(string? after = null, CancellationToken cancellationToken = default) => await GetAllDatedDataAfterAsync<MapDetail>(MapsEndpoint, after, cancellationToken);
        public static async Task GetAllPlaylistsAfterAsync(DataCallback<Playlist> callback, string? after = null, CancellationToken cancellationToken = default) => await GetAllDatedDataAfterAsync(PlaylistsEndpoint, callback, after, cancellationToken);
        public static async Task GetAllMapsAfterAsync(DataCallback<MapDetail> callback, string? after = null, CancellationToken cancellationToken = default) => await GetAllDatedDataAfterAsync(MapsEndpoint, callback, after, cancellationToken);
        public static async Task<List<Playlist>> GetAllPlaylistsBeforeAsync(string? before = null, CancellationToken cancellationToken = default) => await GetAllDatedDataBeforeAsync<Playlist>(PlaylistsEndpoint, before, cancellationToken);
        public static async Task<List<MapDetail>> GetAllMapsBeforeAsync(string? before = null, CancellationToken cancellationToken = default) => await GetAllDatedDataBeforeAsync<MapDetail>(MapsEndpoint, before, cancellationToken);
        public static async Task GetAllPlaylistsBeforeAsync(DataCallback<Playlist> callback, string? before = null, CancellationToken cancellationToken = default) => await GetAllDatedDataBeforeAsync(PlaylistsEndpoint, callback, before, cancellationToken);
        public static async Task GetAllMapsBeforeAsync(DataCallback<MapDetail> callback, string? before = null, CancellationToken cancellationToken = default) => await GetAllDatedDataBeforeAsync(MapsEndpoint, callback, before, cancellationToken);

        public static async Task<List<Playlist>> GetAllPlaylistsAfterAsync(DateTimeOffset? after = null, CancellationToken cancellationToken = default) => await GetAllDatedDataAfterAsync<Playlist>(PlaylistsEndpoint, after?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), cancellationToken);
        public static async Task<List<MapDetail>> GetAllMapsAfterAsync(DateTimeOffset? after = null, CancellationToken cancellationToken = default) => await GetAllDatedDataAfterAsync<MapDetail>(MapsEndpoint, after?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), cancellationToken);
        public static async Task GetAllPlaylistsAfterAsync(DataCallback<Playlist> callback, DateTimeOffset? after = null, CancellationToken cancellationToken = default) => await GetAllDatedDataAfterAsync(PlaylistsEndpoint, callback, after?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), cancellationToken);
        public static async Task GetAllMapsAfterAsync(DataCallback<MapDetail> callback, DateTimeOffset? after = null, CancellationToken cancellationToken = default) => await GetAllDatedDataAfterAsync(MapsEndpoint, callback, after?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), cancellationToken);
        public static async Task<List<Playlist>> GetAllPlaylistsBeforeAsync(DateTimeOffset? before = null, CancellationToken cancellationToken = default) => await GetAllDatedDataBeforeAsync<Playlist>(PlaylistsEndpoint, before?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), cancellationToken);
        public static async Task<List<MapDetail>> GetAllMapsBeforeAsync(DateTimeOffset? before = null, CancellationToken cancellationToken = default) => await GetAllDatedDataBeforeAsync<MapDetail>(MapsEndpoint, before?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), cancellationToken);
        public static async Task GetAllPlaylistsBeforeAsync(DataCallback<Playlist> callback, DateTimeOffset? before = null, CancellationToken cancellationToken = default) => await GetAllDatedDataBeforeAsync(PlaylistsEndpoint, callback, before?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), cancellationToken);
        public static async Task GetAllMapsBeforeAsync(DataCallback<MapDetail> callback, DateTimeOffset? before = null, CancellationToken cancellationToken = default) => await GetAllDatedDataBeforeAsync(MapsEndpoint, callback, before?.ToUniversalTime().ToString(ApiDateFormat, CultureInfo.InvariantCulture), cancellationToken);

        /// <summary>
        /// Gets the details for the specified playlist.
        /// </summary>
        /// <param name="id">The ID of the playlist.</param>
        /// <param name="page">The specific page to fetch, if omitted it will fetch all maps.</param>
        /// <returns>A playlist details object.</returns>
        private static async Task<PlaylistPage> GetPlaylistDetailsAsync(long id, int page, CancellationToken cancellationToken = default)
        {
            return await GetJsonAsync<PlaylistPage>($"https://api.beatsaver.com/playlists/id/{id}/{page}", 3, cancellationToken);
        }

        private static async Task<PlaylistPage> GetPlaylistDetailsAsync(long id, CancellationToken cancellationToken = default)
        {
            int pageNum = 0;
            var playlist = await GetPlaylistDetailsAsync(id, pageNum++, cancellationToken);

            if (playlist.Maps.Count == 0)
            {
                return playlist;
            }

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = await GetPlaylistDetailsAsync(id, pageNum++, cancellationToken);

                playlist.Maps.AddRange(data.Maps);

                if (data.Maps.Count == 0)
                {
                    break;
                }
            }

            return playlist;
        }

        /// <summary>
        /// Gets the details for the specified playlist.
        /// </summary>
        /// <param name="id">The ID of the playlist.</param>
        /// <returns>A playlist details object.</returns>
        public static async Task<PlaylistPage> GetPlaylistDetailsAsync(int id, CancellationToken cancellationToken = default) => await GetPlaylistDetailsAsync(id, cancellationToken);

        /// <summary>
        /// Gets the details for the specified playlist.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <returns>A playlist details object.</returns>
        public static async Task<PlaylistPage> GetPlaylistDetailsAsync(Playlist playlist, CancellationToken cancellationToken = default) => await GetPlaylistDetailsAsync(playlist.PlaylistId, cancellationToken);
    }

}
