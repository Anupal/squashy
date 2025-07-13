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
    public void DummyTest()
    {
        GitCommands git = CommonTestSetup();
    }

    public GitCommands CommonTestSetup()
    {
        Utility.CreateTestRepo(testRepoPath);
        Utility.CreateFileCommits(testRepoPath);
        return new GitCommands(testRepoPath);
    }
}
