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

        rootCmd.Add(directoryOption);
        rootCmd.Add(logCommitsCmd);

        ParseResult parseResult = rootCmd.Parse(args);
        return parseResult.Invoke();
    }
}