using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Dto
{
    public class ApplyScriptProgress
    {
        /// <summary>
        /// the script that we are starting to process or just finished (depending on <see cref="IsStarting"/> 
        /// </summary>
        public Data.Models.Script Script { get; set; }

        /// <summary>
        /// the revision logged. Will be Null if <see cref="IsStarting"/>  is true
        /// </summary>
        public Data.Models.Revision Revision { get; set; }

        /// <summary>
        /// indicates whether we are starting to process a script, or just finished
        /// </summary>
        public bool IsStarting { get; set; }

        public int ScriptsCompleted { get; set; }
        public int ScriptsRemaining { get; set; }

    }
}
