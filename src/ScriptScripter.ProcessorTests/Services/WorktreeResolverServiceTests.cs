using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace ScriptScripter.Processor.Services.Tests
{
    [TestClass]
    public class WorktreeResolverServiceTests
    {
        private System.IO.Abstractions.TestingHelpers.MockFileSystem _mockFileSystem;

        //act input
        private string _inputPath;

        [TestInitialize]
        public void Init()
        {
            _mockFileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
        }

        private Dto.WorktreeResolutionResult Act()
        {
            var service = new WorktreeResolverService(_mockFileSystem);
            return service.ResolveWorktreeCandidatePath(_inputPath);
        }

        [TestMethod]
        public void well_formed_worktree_resolves_to_rerooted_candidate_path()
        {
            //arrange
            var worktreeRoot = @"C:\Code\NaviPay\.claude\worktrees\payment-error-statuses-163b7d";
            var gitDirForWorktree = @"C:\Code\NaviPay\.git\worktrees\payment-error-statuses-163b7d";

            _mockFileSystem.AddFile(worktreeRoot + @"\.git", new System.IO.Abstractions.TestingHelpers.MockFileData("gitdir: " + gitDirForWorktree));
            _mockFileSystem.AddFile(gitDirForWorktree + @"\commondir", new System.IO.Abstractions.TestingHelpers.MockFileData(@"../.."));

            _inputPath = worktreeRoot + @"\Database\Scripts";

            //act
            var result = this.Act();

            //assert
            result.IsWorktree.Should().BeTrue();
            result.WorktreeRoot.Should().Be(worktreeRoot);
            result.MainCheckoutRoot.Should().Be(@"C:\Code\NaviPay");
            result.CandidatePath.Should().Be(@"C:\Code\NaviPay\Database\Scripts");
        }

        [TestMethod]
        public void non_worktree_folder_with_no_git_anywhere_returns_not_a_worktree()
        {
            //arrange
            _inputPath = @"C:\Code\SomeProject\Database\Scripts";

            //act
            var result = this.Act();

            //assert
            result.IsWorktree.Should().BeFalse();
        }

        [TestMethod]
        public void primary_checkout_with_git_folder_returns_not_a_worktree()
        {
            //arrange
            _mockFileSystem.AddDirectory(@"C:\Code\NaviPay\.git");
            _inputPath = @"C:\Code\NaviPay\Database\Scripts";

            //act
            var result = this.Act();

            //assert
            result.IsWorktree.Should().BeFalse();
        }

        [TestMethod]
        public void malformed_gitdir_line_returns_not_a_worktree_without_throwing()
        {
            //arrange
            var worktreeRoot = @"C:\Code\NaviPay\.claude\worktrees\payment-error-statuses-163b7d";
            _mockFileSystem.AddFile(worktreeRoot + @"\.git", new System.IO.Abstractions.TestingHelpers.MockFileData("not a valid gitdir file"));

            _inputPath = worktreeRoot + @"\Database\Scripts";

            //act
            Dto.WorktreeResolutionResult result = null;
            Action act = () => result = this.Act();

            //assert
            act.Should().NotThrow();
            result.IsWorktree.Should().BeFalse();
        }

        [TestMethod]
        public void missing_commondir_file_returns_not_a_worktree_without_throwing()
        {
            //arrange
            var worktreeRoot = @"C:\Code\NaviPay\.claude\worktrees\payment-error-statuses-163b7d";
            var gitDirForWorktree = @"C:\Code\NaviPay\.git\worktrees\payment-error-statuses-163b7d";

            _mockFileSystem.AddFile(worktreeRoot + @"\.git", new System.IO.Abstractions.TestingHelpers.MockFileData("gitdir: " + gitDirForWorktree));
            //note: commondir file intentionally not added

            _inputPath = worktreeRoot + @"\Database\Scripts";

            //act
            Dto.WorktreeResolutionResult result = null;
            Action act = () => result = this.Act();

            //assert
            act.Should().NotThrow();
            result.IsWorktree.Should().BeFalse();
        }

        [TestMethod]
        public void worktree_several_directories_deep_preserves_relative_path()
        {
            //arrange
            var worktreeRoot = @"C:\Code\NaviPay\.claude\worktrees\payment-error-statuses-163b7d";
            var gitDirForWorktree = @"C:\Code\NaviPay\.git\worktrees\payment-error-statuses-163b7d";

            _mockFileSystem.AddFile(worktreeRoot + @"\.git", new System.IO.Abstractions.TestingHelpers.MockFileData("gitdir: " + gitDirForWorktree));
            _mockFileSystem.AddFile(gitDirForWorktree + @"\commondir", new System.IO.Abstractions.TestingHelpers.MockFileData(@"../.."));

            _inputPath = worktreeRoot + @"\Database\Scripts\SubFolder\Deeper";

            //act
            var result = this.Act();

            //assert
            result.IsWorktree.Should().BeTrue();
            result.CandidatePath.Should().Be(@"C:\Code\NaviPay\Database\Scripts\SubFolder\Deeper");
        }
    }
}
