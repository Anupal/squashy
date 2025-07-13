using NUnit.Framework;
using Squashy;

namespace Squashy.Tests;

public class E2eTests
{
    private string testRepoPath = Path.Combine(Environment.CurrentDirectory, ".test-repo");
    private string squashMessage = "squashed commit";
    
    [SetUp]
    public void Setup()
    { 
    }

    [Test]
    public void Test_Log_Output()
    {
        GitCommands git = CommonTestSetup();
        
        // capture console output in a string
        var sw = new StringWriter();
        Console.SetOut(sw);

        git.Log(10);

        var consoleOutput = sw.ToString();

        // reset console back to Stdout
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        
        // Split output lines and skip header
        var lines = consoleOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var commitLines = lines.Skip(2).ToArray();

        var expectedMessages = new[]
        {
            "delete file 3",
            "delete file 2",
            "delete file 1",
            "modify file 3",
            "modify file 2",
            "modify file 1",
            "create file 3",
            "create file 2",
            "create file 1"
        };

        Assert.That(commitLines.Length, Is.EqualTo(expectedMessages.Length), "Unexpected number of commit lines.");
        for (int i = 0; i < expectedMessages.Length; i++)
        {
            StringAssert.Contains(expectedMessages[i], commitLines[i], $"Message mismatch at line {i + 3}");
        }        
    }
    
    // Test squashing head to middle
    [Test]
    public void Test_SquashWithHead()
    {
        GitCommands git = CommonTestSetup();

        var originalShas = Utility.GetAllCommitHashes(testRepoPath);
        // We'll squash commits between last (HEAD) and eight commits
        string firstCommitSha = originalShas[0];
        string secondCommitSha = originalShas[7];

        git.Squash(firstCommitSha, secondCommitSha, false, squashMessage);

        // We only expect 'create file 1' and 'squashed commit' messages to be
        // present in the log
        var newCommitMessages = Utility.GetAllCommitMessages(testRepoPath);
        Assert.That(newCommitMessages.Count, Is.EqualTo(2), "Unexpected number of commit lines.");
        Assert.That(newCommitMessages[0], Is.EqualTo("squashed commit"), "First commit is not 'squashed commit'");
        Assert.That(newCommitMessages[1], Is.EqualTo("create file 1"), "Second commit is not 'create file 1");
    }
    
    // Test squashing entire commit history
    [Test]
    public void Test_SquashWithHeadAndRoot()
    {
        GitCommands git = CommonTestSetup();

        var originalShas = Utility.GetAllCommitHashes(testRepoPath);
        // We'll squash commits between last (HEAD) and oldest commit
        string firstCommitSha = originalShas[0];
        string secondCommitSha = originalShas[8];

        git.Squash(firstCommitSha, secondCommitSha, false, squashMessage);

        // We only expect 'squashed commit' messages to be in the log
        var newCommitMessages = Utility.GetAllCommitMessages(testRepoPath);
        Assert.That(newCommitMessages.Count, Is.EqualTo(1), "Unexpected number of commit lines.");
        Assert.That(newCommitMessages[0], Is.EqualTo("squashed commit"), "First commit is not 'squashed commit'");
    }
    
    // Test squashing middle commit to root
    [Test]
    public void Test_SquashWithRoot()
    {
        GitCommands git = CommonTestSetup();

        var originalShas = Utility.GetAllCommitHashes(testRepoPath);
        // We'll squash commits between last (HEAD) and eight commits
        string firstCommitSha = originalShas[1];
        string secondCommitSha = originalShas[8];
        git.Squash(firstCommitSha, secondCommitSha, false, squashMessage);

        // We only expect 'delete file 3' and 'squashed commit' messages to be
        // present in the log
        var newCommitMessages = Utility.GetAllCommitMessages(testRepoPath);
        Assert.That(newCommitMessages.Count, Is.EqualTo(2), "Unexpected number of commit lines.");
        Assert.That(newCommitMessages[0], Is.EqualTo("delete file 3"), "Frist commit is not 'delete file 3");
        Assert.That(newCommitMessages[1], Is.EqualTo("squashed commit"), "Second commit is not 'squashed commit'");
    }
    
    // Test squashing block of commit between head and root
    [Test]
    public void TestSquash()
    {
        GitCommands git = CommonTestSetup();

        var originalShas = Utility.GetAllCommitHashes(testRepoPath);
        // We'll squash commits between last (HEAD) and eight commits
        string firstCommitSha = originalShas[1];
        string secondCommitSha = originalShas[7];

        git.Squash(firstCommitSha, secondCommitSha, false, squashMessage);

        // We only expect 'delete file 3', 'create file 1' and 'squashed commit' messages to be
        // present in the log
        var newCommitMessages = Utility.GetAllCommitMessages(testRepoPath);
        Assert.That(newCommitMessages.Count, Is.EqualTo(3), "Unexpected number of commit lines.");
        Assert.That(newCommitMessages[0], Is.EqualTo("delete file 3"), "First commit is not 'delete file 3");
        Assert.That(newCommitMessages[1], Is.EqualTo("squashed commit"), "Second commit is not 'squashed commit'");
        Assert.That(newCommitMessages[2], Is.EqualTo("create file 1"), "Third commit is not 'create file 1");

    }

    public GitCommands CommonTestSetup()
    {
        Utility.CreateTestRepo(testRepoPath);
        Utility.CreateFileCommits(testRepoPath);
        return new GitCommands(testRepoPath);
    }
}
