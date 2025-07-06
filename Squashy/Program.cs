using System.CommandLine;

class Program
{
    static int Main(string[] args)
    {
        GitCommands git = new();
        RootCommand rootCmd = new("Squashy: Simple commandline git squasher");

        Command logCommitsCmd = new("log", "List last N commits");
        Option<string> directoryOption = new("-d")
        {
            Description = "Git directory",
            DefaultValueFactory = parseResult => ".",
        };
        Option<int> numCommitOptions = new("-n")
        {
            Description = "Number of commits",
            DefaultValueFactory = parseResult => 10,
        };
        logCommitsCmd.Add(directoryOption);
        logCommitsCmd.Add(numCommitOptions);
        logCommitsCmd.SetAction(parseResult =>
        {
            git.Log(parseResult.GetValue(directoryOption), parseResult.GetValue(numCommitOptions));
            return 0;
        });

        rootCmd.Add(logCommitsCmd);

        ParseResult parseResult = rootCmd.Parse(args);
        return parseResult.Invoke();
    }
}