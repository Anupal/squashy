using NUnit.Framework;
using Squashy;

namespace Squashy.Tests;

public class E2eTests
{
    private string testRepoPath = Path.Combine(Environment.CurrentDirectory, ".test-repo");
    
    [SetUp]
    public void Setup()
    { 
    }

    [Test]
    public void Git_Log_Output()
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

    public GitCommands CommonTestSetup()
    {
        Utility.CreateTestRepo(testRepoPath);
        Utility.CreateFileCommits(testRepoPath);
        return new GitCommands(testRepoPath);
    }
}
