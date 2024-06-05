using CommandLine;

namespace BSSlurper.CLI.Commands
{
    [Verb("update", true, HelpText = "Updates the local mirror.")]
    public class UpdateCommandOptions
    {
        [Option('p', "data-path", Default = null, HelpText = "The base path to store the data in.")]
        public DirectoryInfo? DataPath { get; set; }

        [Option("update-all", Default = false, HelpText = "If this option is used, all playlists and maps will be updated.")]
        public bool FullUpdate { get; set; }

        [Option("skip-previews", Default = false, HelpText = "Skip downloading the map previews.")]
        public bool SkipPreviews { get; set; }

        [Option("skip-map-covers", Default = false, HelpText = "Skip downloading cover art for maps.")]
        public bool SkipMapCovers { get; set; }

        [Option("skip-playlist-covers", Default = false, HelpText = "Skip downloading playlist cover art.")]
        public bool SkipPlaylistCovers { get; set; }

        [Option("skip-playlists", Default = false, HelpText = "Skip downloading playlists.")]
        public bool SkipPlaylists { get; set; }
    }
}
