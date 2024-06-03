using BSSlurper.CLI.Commands;
using CommandLine;

object? parsedOptions = null;

var token = new CancellationTokenSource();

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;

    token.Cancel();
};

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
            try
            {
                await updater.Execute(token.Token);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Cancelled!");
            }
        }
        break;
}