using LibGit2Sharp;

public class GitCommands
{
    private string directory;
    private Repository repo;


    public GitCommands(string directory)
    {
        Console.WriteLine($"Loading repo from: {directory}");
        this.directory = directory;
        this.repo = new Repository(directory);
    }

    public void Log(int numCommits)
    {
        printCommitsTable(getCommits(numCommits), true);
    }

    public void Squash(string firstCommitSha, string secondCommitSha, bool dryRun)
    {
        using var repo = new Repository(directory);
        var firstCommit = fetchCommitBySha(repo, firstCommitSha);
        var secondCommit = fetchCommitBySha(repo, secondCommitSha);

        if (firstCommit == null || secondCommit == null) { return; }


        if (dryRun)
        {
            squashDryRun(getCommits(), firstCommitSha, secondCommitSha);
        }
    }

    private IEnumerable<Commit> getCommits()
    {
        var filter = new CommitFilter { SortBy = CommitSortStrategies.Topological };
        return repo.Commits.QueryBy(filter);
    }

    private IEnumerable<Commit> getCommits(int numCommits)
    {
        var filter = new CommitFilter { SortBy = CommitSortStrategies.Topological };
        return repo.Commits.QueryBy(filter).Take(numCommits);
    }

    private void printCommitsTable(IEnumerable<Commit> commitList, bool displayHeader = false)
    {
        if (displayHeader)
        {
            Console.WriteLine("{0,-7} {1,-20} {2,-50}", "Hash", "Author", "Message");
            Console.WriteLine(new string('-', 40));
        }

        foreach (Commit commit in commitList)
        {
            string shortHash = commit.Sha.Substring(0, 7);
            string author = commit.Author.Name;
            string message = commit.MessageShort;

            // Manually truncate to match desired max lengths
            if (author.Length > 20) author = author.Substring(0, 20);
            if (message.Length > 50) message = message.Substring(0, 50);
            Console.WriteLine("{0,-7} {1,-20} {2,-50}", shortHash, author, message);
        }
    }
    
    /// <summary>
    ///  Checks if the commit exists for the given SHA and is present in the current branch.
    /// </summary>
    /// <returns>The retrieved <c>Commit</c> object or <c>null</c> if not found or not present in the current branch.</returns>
    private Commit fetchCommitBySha(Repository repo, string commitSha)
    {
        if (commitSha.Length != 7 && commitSha.Length != 40)
        {
            Console.WriteLine($"Invalid commit SHA length for '{commitSha}'. Expected 7 or 40 characters, got {commitSha.Length}");
            return null;
        }

        var commit = repo.Lookup<Commit>(commitSha);
        if (commit == null)
        {
            Console.WriteLine($"Commit '{commitSha}' does not exist!");
            return null;
        }

        // check if commit is in the current branch
        var currentBranch = repo.Head;
        var existsInBranch = repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = currentBranch }).Any(c => c.Sha.StartsWith(commitSha));
        if (!existsInBranch)
        {
            Console.WriteLine($"Commit '{commitSha}' does not exist in the current branch");
            return null;
        }

        return commit;
    }

    private void squashDryRun(IEnumerable<Commit> commits, string firstCommitSha, string secondCommitSha)
    {

    }
}