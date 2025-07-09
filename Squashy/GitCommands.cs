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

    public void Squash(string firstCommitSha, string secondCommitSha, bool dryRun, string message)
    {
        using var repo = new Repository(directory);
        var firstCommit = fetchCommitBySha(repo, firstCommitSha);
        var secondCommit = fetchCommitBySha(repo, secondCommitSha);

        if (firstCommit == null || secondCommit == null) { return; }

        // swap to ensure firstCommit always comes before secondCommit
        if (!isAncestor(repo, firstCommit, secondCommit))
        {
            (firstCommit, secondCommit) = (secondCommit, firstCommit);
        }

        if (dryRun)
        {
            squashDryRun(repo, firstCommit, secondCommit, message);
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
    /// Checks if the commit exists for the given SHA and is present in the current branch.
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="commitSha"></param>
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

    /// <summary>
    /// Dry run for squashing commits in the given range (inclusive).
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="firstCommit"></param>
    /// <param name="secondCommit"></param>
    /// <param name="message"></param>
    private void squashDryRun(Repository repo, Commit firstCommit, Commit secondCommit, string message)
    {
        // if secondCommit is not the head, we need to print commits that come after it
        if (!isHeadCommit(repo, secondCommit))
        {
            var commitsAfterSecond = repo.Commits.QueryBy(
                new CommitFilter
                {
                    IncludeReachableFrom = repo.Head,
                    ExcludeReachableFrom = secondCommit,
                    SortBy = CommitSortStrategies.Topological
                }
            ).SkipWhile(commit => commit.Sha == secondCommit.Sha);

            printCommitsTable(commitsAfterSecond);
        }
        Console.WriteLine($"<squashed commit> '{message}'");

        // get all commits before the firstCommit
        var commitsBeforeFirst = repo.Commits.QueryBy(
            new CommitFilter
            {
                IncludeReachableFrom = firstCommit,
                SortBy = CommitSortStrategies.Topological
            }
        ).SkipWhile(commit => commit.Sha == firstCommit.Sha);
        printCommitsTable(commitsBeforeFirst);
    }

    /// <summary>
    /// Checks if commit is the head of current branch.
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="commit"></param>
    /// <returns>Returns <c>true</c> if commit is the head of current branch.</returns>
    private bool isHeadCommit(Repository repo, Commit commit)
    {
        return repo.Head.Tip.Sha == commit.Sha;
    }

    /// <summary>
    /// Checks if firstCommit comes before secondCommit in the tree.
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="firstCommit"></param>
    /// <param name="secondCommit"></param>
    /// <returns>Returns <c>true</c> if firstCommit is the ancestor of secondCommit.</returns>
    bool isAncestor(Repository repo, Commit firstCommit, Commit secondCommit)
    {
        return repo.ObjectDatabase.FindMergeBase(firstCommit, secondCommit)?.Sha == firstCommit.Sha;
    }
}