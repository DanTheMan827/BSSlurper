using BSSlurper.Core.BeatSaver.API;
using BSSlurper.Core.BeatSaver.API.Models;
using BSSlurper.Core.Database;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BSSlurper.CLI.Commands
{
    /// <summary>
    /// The UpdateCommand class handles the updating of maps and playlists from the BeatSaver API.
    /// </summary>
    internal class UpdateCommand : IDisposable
    {
        private readonly SlurpedContext db;
        private readonly bool fullUpdate;
        private readonly string dataPath;
        private readonly string mapsPath;
        private readonly string avatarsPath;
        private readonly string coversPath;
        private readonly string previewsPath;
        private readonly string playlistImagesPath;
        private readonly string playlist512ImagesPath;
        private readonly string missingPath;

        /// <summary>
        /// Initializes a new instance of the UpdateCommand class with specified options.
        /// </summary>
        /// <param name="options">Options for the update command, containing paths and other settings.</param>
        public UpdateCommand(UpdateCommandOptions options)
        {
            fullUpdate = options.FullUpdate;
            dataPath = options.DataPath != null ? Path.GetFullPath(options.DataPath.FullName) : Path.GetFullPath("data");
            mapsPath = Path.Combine(dataPath, "maps");
            avatarsPath = Path.Combine(dataPath, "avatars");
            coversPath = Path.Combine(dataPath, "map-covers");
            previewsPath = Path.Combine(dataPath, "map-previews");
            playlistImagesPath = Path.Combine(dataPath, "playlist-covers");
            playlist512ImagesPath = Path.Combine(dataPath, "playlist-covers-512");
            missingPath = Path.Combine(dataPath, "404.txt");

            Directory.CreateDirectory(dataPath);

            db = new SlurpedContext(Path.Combine(dataPath, "beatsaver.db"));
        }

        /// <summary>
        /// Gets the date of the oldest map in the database.
        /// </summary>
        /// <returns>The date of the oldest map, or null if no maps are found.</returns>
        DateTimeOffset? GetOldestMapDate() => db.Maps.OrderBy(m => m.UpdatedAt).Select(m => m.UpdatedAt).FirstOrDefault();

        /// <summary>
        /// Gets the date of the newest map in the database.
        /// </summary>
        /// <returns>The date of the newest map, or null if no maps are found.</returns>
        DateTimeOffset? GetNewestMapDate() => db.Maps.OrderByDescending(m => m.UpdatedAt).Select(m => m.UpdatedAt).FirstOrDefault();

        /// <summary>
        /// Gets the date of the oldest playlist in the database.
        /// </summary>
        /// <returns>The date of the oldest playlist, or null if no playlists are found.</returns>
        DateTimeOffset? GetOldestPlaylistDate() => db.Playlists.OrderBy(m => m.UpdatedAt).Select(m => m.UpdatedAt).FirstOrDefault();

        /// <summary>
        /// Gets the date of the newest playlist in the database.
        /// </summary>
        /// <returns>The date of the newest playlist, or null if no playlists are found.</returns>
        DateTimeOffset? GetNewestPlaylistDate() => db.Playlists.OrderByDescending(m => m.UpdatedAt).Select(m => m.UpdatedAt).FirstOrDefault();

        /// <summary>
        /// Downloads a file from the specified URI to the specified file path, with retry logic.  If a 404 has been encountered, it will be logged to a file.
        /// </summary>
        /// <param name="uri">The URI of the file to download.</param>
        /// <param name="fileName">The local file path to save the downloaded file.</param>
        /// <param name="maxRetries">The maximum number of retry attempts (default is 3).</param>
        async Task DownloadAsync(Uri uri, string fileName, int maxRetries = 3)
        {
            int retryCount = 0;
            var destinationDirectory = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            while (retryCount < maxRetries)
            {
                try
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    using (var client = new WebClient())
                    {
                        await client.DownloadFileTaskAsync(uri, fileName);
                        Console.WriteLine($"Downloaded: {uri}");
                        return; // Success, exit the loop
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    if (retryCount >= maxRetries)
                    {
                        if (ex is WebException webException && webException.Response is HttpWebResponse response)
                        {
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                File.AppendAllText(missingPath, $"{uri}\n");
                            }
                            else
                            {
                                throw;
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Downloads all versions of the specified maps.
        /// </summary>
        /// <param name="versions">The map versions to download.</param>
        async Task DownloadVersionsAsync(params MapVersion[] versions)
        {
            foreach (var version in versions)
            {
                var mapPath = Path.Combine(mapsPath, $"{version.Hash}.zip");
                var previewPath = Path.Combine(previewsPath, $"{version.Hash}.mp3");
                var coverPath = Path.Combine(coversPath, $"{version.Hash}.jpg");

                if (!File.Exists(mapPath))
                {
                    await DownloadAsync(version.DownloadUrl, mapPath);
                }

                if (!File.Exists(previewPath))
                {
                    await DownloadAsync(version.PreviewUrl, previewPath);
                }

                if (!File.Exists(coverPath))
                {
                    await DownloadAsync(version.CoverUrl, coverPath);
                }
            }
        }

        /// <summary>
        /// Processes a playlist by downloading its details and associated images, and updating the database.
        /// </summary>
        /// <param name="playlist">The playlist to process.</param>
        /// <param name="clearChangeTracker">Whether to clear the database change tracker after processing (default is false).</param>
        async Task ProcessPlaylistAsync(Playlist playlist, bool clearChangeTracker = false)
        {
            var page = await ApiClient.GetPlaylistDetailsAsync(playlist);
            await db.GetUpdatedPlaylistAsync(page);

            if (page?.Playlist?.PlaylistImage != null)
            {
                var image = page.Playlist.PlaylistImage;

                await DownloadAsync(image, Path.Combine(playlistImagesPath, Path.GetFileName(image.LocalPath)));
            }

            if (page?.Playlist?.PlaylistImage512 != null)
            {
                var image = page.Playlist.PlaylistImage512;

                await DownloadAsync(image, Path.Combine(playlist512ImagesPath, Path.GetFileName(image.LocalPath)));
            }

            var newestPlaylistMapDate = page.Maps.Select(m => m.Map.UpdatedAt).OrderDescending().FirstOrDefault();
            var newestDatabaseMapDate = GetNewestMapDate();

            if ((page.Maps.Count > 0 && newestDatabaseMapDate == null) || (newestPlaylistMapDate != null && newestDatabaseMapDate != null && newestPlaylistMapDate > newestDatabaseMapDate))
            {
                await UpdateMapsAsync();
            }

            await DownloadVersionsAsync(page.Maps.SelectMany(m => m.Map.Versions).ToArray());
            await db.SaveChangesAsync();

            if (clearChangeTracker)
            {
                db.ChangeTracker.Clear();
            }
        }

        /// <summary>
        /// Processes a map by downloading its details and versions, and updating the database.
        /// </summary>
        /// <param name="map">The map to process.</param>
        /// <param name="clearChangeTracker">Whether to clear the database change tracker after processing (default is false).</param>
        async Task ProcessMapAsync(MapDetail map, bool clearChangeTracker = false)
        {
            await db.GetUpdatedMapDetailAsync(map);
            await DownloadVersionsAsync(map.Versions.ToArray());
            await db.SaveChangesAsync();

            if (clearChangeTracker)
            {
                db.ChangeTracker.Clear();
            }
        }

        /// <summary>
        /// Updates all maps from the BeatSaver API by fetching new and updated maps and processing them.
        /// </summary>
        async Task UpdateMapsAsync()
        {
            await ApiClient.GetAllMapsBeforeAsync(ProcessMapAsync, fullUpdate ? null : GetOldestMapDate());

            while (true)
            {
                var data = await ApiClient.GetAllMapsAfterAsync(GetNewestMapDate());

                if (data.Count == 0)
                {
                    break;
                }

                data.Reverse();

                foreach (var item in data.Select((map, index) => (map, index)))
                {
                    await ProcessMapAsync(item.map, item.index == data.Count - 1);
                }
            }
        }

        /// <summary>
        /// Updates all playlists from the BeatSaver API by fetching new and updated playlists and processing them.
        /// </summary>
        async Task UpdatePlaylistsAsync()
        {
            await ApiClient.GetAllPlaylistsBeforeAsync(ProcessPlaylistAsync, fullUpdate ? null : GetOldestPlaylistDate());

            while (true)
            {
                var data = await ApiClient.GetAllPlaylistsAfterAsync(GetNewestPlaylistDate());

                if (data.Count == 0)
                {
                    break;
                }

                data.Reverse();

                foreach (var item in data.Select((playlist, index) => (playlist, index)))
                {
                    await ProcessPlaylistAsync(item.playlist, item.index == data.Count - 1);
                }
            }
        }

        /// <summary>
        /// Executes the update command by updating maps and playlists.
        /// </summary>
        public async Task Execute()
        {
            await db.Database.MigrateAsync();

            await UpdateMapsAsync();
            await UpdatePlaylistsAsync();
        }

        /// <summary>
        /// Disposes the UpdateCommand instance, closing the database connection.
        /// </summary>
        public void Dispose()
        {
            try
            {
                db.Database.CloseConnection();
            }
            catch (Exception) { }

            db.Dispose();
        }
    }
}
