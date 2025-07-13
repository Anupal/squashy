using LibGit2Sharp; 
using System.IO;

namespace Squashy.Tests;

static class Utility
{
    public static void CreateTestRepo(string testRepoPath) {
        Console.WriteLine($"Setting up test repo '{testRepoPath}'"); 
        // Delete repo if it already exists
        if (Directory.Exists(testRepoPath))
        {
            Directory.Delete(testRepoPath, recursive: true);
        }
        Directory.CreateDirectory(testRepoPath);
        Repository.Init(testRepoPath);
    }

    public static void CreateFileCommits(string testRepoPath, int numOfFiles = 3) {
        Repository repo = new Repository(testRepoPath);
        repo.Refs.UpdateTarget("HEAD", "refs/heads/main");

        // Add files
        for (int i = 1; i <= numOfFiles; i++) {
            string filePath = Path.Combine(testRepoPath, $"file{i}.txt");
            File.WriteAllText(filePath, $"file{i} content");

            var sig = new Signature($"Test user {i}", $"testuser{i}@test.com", DateTimeOffset.Now);
            Commands.Stage(repo, filePath);
            repo.Commit($"create file {i}", sig, sig);
        }

        // Modify files
        for (int i = 1; i <= numOfFiles; i++) {
            string filePath = Path.Combine(testRepoPath, $"file{i}.txt");
            File.AppendAllText(filePath, " modified..");
            
            var sig = new Signature($"Test user {i}", $"testuser{i}@test.com", DateTimeOffset.Now);
            Commands.Stage(repo, filePath);
            repo.Commit($"modify file {i}", sig, sig);
        }

        // Delete files
        for (int i = 1; i <= numOfFiles; i++) {
            string filePath = Path.Combine(testRepoPath, $"file{i}.txt");
            File.Delete(filePath);
            
            var sig = new Signature($"Test user {i}", $"testuser{i}@test.com", DateTimeOffset.Now);
            Commands.Stage(repo, filePath);
            repo.Commit($"delete file {i}", sig, sig);
        }
    }
}
