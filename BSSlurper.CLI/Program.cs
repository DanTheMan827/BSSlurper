using BSSlurper.CLI.Commands;
using CommandLine;

object? parsedOptions = null;

Parser.Default.ParseArguments<UpdateCommandOptions>(args)
    .MapResult(
        (UpdateCommandOptions options) => parsedOptions = options,
        errors => 1
    );

switch (parsedOptions)
{
    case UpdateCommandOptions options:
        using (var updater = new UpdateCommand(options))
        {
            await updater.Execute();
        }
        break;
}