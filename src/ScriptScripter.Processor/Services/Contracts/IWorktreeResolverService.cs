using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services.Contracts
{
    public interface IWorktreeResolverService
    {
        /// <summary>
        /// Walks up from <paramref name="path"/> looking for a directory containing a `.git` file (not folder -
        /// that marks a worktree checkout). When found, resolves the main checkout's root via the file's
        /// `gitdir:` line and that gitdir's `commondir` file, then re-roots <paramref name="path"/> under it.
        /// </summary>
        Dto.WorktreeResolutionResult ResolveWorktreeCandidatePath(string path);
    }
}
