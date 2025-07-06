using LibGit2Sharp;

public static class GitCommands
{
    public static void DisplayCommits(string directory, int numCommits)
    {
        using var repo = new Repository(directory);
        var filter = new CommitFilter { SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Reverse };

        foreach (Commit commit in repo.Commits.QueryBy(filter).Take(numCommits))
        {
            Console.WriteLine($"commit {commit.Id} - {commit.Message[..^1]}");
        }
    }
}