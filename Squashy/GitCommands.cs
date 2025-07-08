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

    public IEnumerable<Commit> getCommits()
    {
        var filter = new CommitFilter { SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Reverse };
        return repo.Commits.QueryBy(filter);
    }

    public IEnumerable<Commit> getCommits(int numCommits)
    {
        var filter = new CommitFilter { SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Reverse };
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
}