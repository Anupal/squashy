using LibGit2Sharp;

public static class GitCommands
{
    public static void DisplayCommits(string directory, int numCommits)
    {
        using var repo = new Repository(directory);
        foreach (Commit commit in repo.Commits.Take(numCommits))
        {
            Console.WriteLine($"commit {commit.Id} - {commit.Message[..^1]}");
        }
    }
}