using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services
{
    public class WorktreeResolverService : Contracts.IWorktreeResolverService
    {
        private readonly System.IO.Abstractions.IFileSystem _fileSystem;

        public WorktreeResolverService(System.IO.Abstractions.IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Dto.WorktreeResolutionResult ResolveWorktreeCandidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Dto.WorktreeResolutionResult.NotAWorktree();

            var searchedPath = path;
            var currentDir = _fileSystem.File.Exists(searchedPath) ? _fileSystem.Path.GetDirectoryName(searchedPath) : searchedPath;

            while (!string.IsNullOrEmpty(currentDir))
            {
                var gitFilePath = _fileSystem.Path.Combine(currentDir, ".git");

                //note: File.Exists is false for a directory, so a primary checkout's ".git" *folder* is correctly skipped here
                if (_fileSystem.File.Exists(gitFilePath))
                    return this.ResolveFromGitFile(gitFilePath, worktreeRoot: currentDir, searchedPath: searchedPath);

                currentDir = _fileSystem.Path.GetDirectoryName(currentDir);
            }

            return Dto.WorktreeResolutionResult.NotAWorktree();
        }

        private Dto.WorktreeResolutionResult ResolveFromGitFile(string gitFilePath, string worktreeRoot, string searchedPath)
        {
            var privateGitDir = this.ReadGitDirLine(gitFilePath);
            if (string.IsNullOrEmpty(privateGitDir))
                return Dto.WorktreeResolutionResult.NotAWorktree();

            var commondirFilePath = _fileSystem.Path.Combine(privateGitDir, "commondir");
            if (!_fileSystem.File.Exists(commondirFilePath))
                return Dto.WorktreeResolutionResult.NotAWorktree();

            var commondirContents = _fileSystem.File.ReadAllText(commondirFilePath).Trim();
            if (string.IsNullOrEmpty(commondirContents))
                return Dto.WorktreeResolutionResult.NotAWorktree();

            var mainGitDir = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(privateGitDir, commondirContents));
            var mainCheckoutRoot = _fileSystem.Path.GetDirectoryName(mainGitDir);
            if (string.IsNullOrEmpty(mainCheckoutRoot))
                return Dto.WorktreeResolutionResult.NotAWorktree();

            var relativePath = searchedPath.Length > worktreeRoot.Length ? searchedPath.Substring(worktreeRoot.Length).TrimStart('\\', '/') : "";
            var candidatePath = _fileSystem.Path.Combine(mainCheckoutRoot, relativePath);

            return Dto.WorktreeResolutionResult.Worktree(worktreeRoot: worktreeRoot, mainCheckoutRoot: mainCheckoutRoot, candidatePath: candidatePath);
        }

        private string ReadGitDirLine(string gitFilePath)
        {
            var gitFileContents = _fileSystem.File.ReadAllText(gitFilePath);

            var gitDirLine = gitFileContents
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(line => line.Trim())
                .FirstOrDefault(line => line.StartsWith("gitdir:", StringComparison.OrdinalIgnoreCase));

            if (gitDirLine == null)
                return null;

            return gitDirLine.Substring("gitdir:".Length).Trim();
        }
    }
}
