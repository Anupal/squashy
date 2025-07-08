using System.CommandLine;

class Program
{
    static int Main(string[] args)
    {
        RootCommand rootCmd = new("Squashy: Simple commandline git squasher");

        Option<string> directoryOption = new("-d")
        {
            Description = "Git directory",
            DefaultValueFactory = parseResult => ".",
        };

        Command logCommitsCmd = new("log", "List last N commits");
        Option<int> numCommitOptions = new("-n")
        {
            Description = "Number of commits",
            DefaultValueFactory = parseResult => 10,
        };
        logCommitsCmd.Add(numCommitOptions);
        logCommitsCmd.SetAction(parseResult =>
        {
            GitCommands git = new(parseResult.GetValue(directoryOption));
            git.Log(parseResult.GetValue(numCommitOptions));
            return 0;
        });

        Command squashCommitsCmd = new(
            "squash",
            "Squash all commits between the specified start and end commits (inclusive)."
        );
        var firstCommitArg = new Argument<string>("firstCommit")
        {
            Description = "The SHA of the first (older) commit."
        };
        var secondCommitArg = new Argument<string>("secondCommit")
        {
            Description = "The SHA of the second (newer) commit."
        };
        Option<bool> dryRunOption = new("--dry-run", "-x")
        {
            Description = "To show expected output.",
            DefaultValueFactory = parseResult => false,
        };
        squashCommitsCmd.Add(firstCommitArg);
        squashCommitsCmd.Add(secondCommitArg);
        squashCommitsCmd.Add(dryRunOption);
        squashCommitsCmd.SetAction(parseResult =>
        {
            GitCommands git = new(parseResult.GetValue(directoryOption));
            git.Squash(
                parseResult.GetValue(firstCommitArg),
                parseResult.GetValue(secondCommitArg),
                parseResult.GetValue(dryRunOption));
            return 0;
        });

        rootCmd.Add(directoryOption);
        rootCmd.Add(logCommitsCmd);
        rootCmd.Add(squashCommitsCmd);

        ParseResult parseResult = rootCmd.Parse(args);
        return parseResult.Invoke();
    }
}