using CommandLine;

namespace BSSlurper.CLI.Commands
{
    [Verb("update", true, HelpText = "Updates the local mirror.")]
    public class UpdateCommandOptions
    {
        [Option('p', "data-path", Default = null, HelpText = "The base path to store the data in.")]
        public DirectoryInfo? DataPath { get; set; }

        [Option("full", Default = false, HelpText = "If this option is used, all playlists and maps will be updated.")]
        public bool FullUpdate { get; set; }
    }
}
