using System.CommandLine;

class Program
{
    static int Main(string[] args)
    {
        GitCommands git = new();
        RootCommand rootCmd = new("Squashy: Simple commandline git squasher");

        Command listCommitsCmd = new("list", "List last N commits");
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
        listCommitsCmd.Add(directoryOption);
        listCommitsCmd.Add(numCommitOptions);
        listCommitsCmd.SetAction(parseResult =>
        {
            git.DisplayCommits(parseResult.GetValue(directoryOption), parseResult.GetValue(numCommitOptions));
            return 0;
        });

        rootCmd.Add(listCommitsCmd);

        ParseResult parseResult = rootCmd.Parse(args);
        return parseResult.Invoke();
    }
}