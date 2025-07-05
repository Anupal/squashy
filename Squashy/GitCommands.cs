using LibGit2Sharp;

public class GitCommands
{
    public void DisplayCommits(string directory, int numCommits)
    {
        using var repo = new Repository(directory);
        var filter = new CommitFilter { SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Reverse };
        printCommitsTable(repo.Commits.QueryBy(filter).Take(numCommits), true);
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