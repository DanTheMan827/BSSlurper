using CommandLine;

namespace BSSlurper.CLI.Commands
{
    [Verb("update", true, HelpText = "Updates the local mirror.")]
    public class UpdateCommandOptions
    {
        [Option('p', "data-path", Default = null)]
        public DirectoryInfo? DataPath { get; set; }
    }
}
