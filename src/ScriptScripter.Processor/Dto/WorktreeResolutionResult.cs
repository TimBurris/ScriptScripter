using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Dto
{
    public class WorktreeResolutionResult
    {
        public bool IsWorktree { get; set; }
        /// <summary>
        /// The directory (containing the `.git` file) that was found while walking up from the searched path.
        /// </summary>
        public string WorktreeRoot { get; set; }
        /// <summary>
        /// The root of the main checkout that the worktree's `.git` file resolves back to.
        /// </summary>
        public string MainCheckoutRoot { get; set; }
        /// <summary>
        /// The searched path, re-rooted under <see cref="MainCheckoutRoot"/> in place of <see cref="WorktreeRoot"/>.
        /// </summary>
        public string CandidatePath { get; set; }

        public static WorktreeResolutionResult NotAWorktree()
        {
            return new Dto.WorktreeResolutionResult() { IsWorktree = false };
        }

        public static WorktreeResolutionResult Worktree(string worktreeRoot, string mainCheckoutRoot, string candidatePath)
        {
            return new Dto.WorktreeResolutionResult()
            {
                IsWorktree = true,
                WorktreeRoot = worktreeRoot,
                MainCheckoutRoot = mainCheckoutRoot,
                CandidatePath = candidatePath,
            };
        }
    }
}
