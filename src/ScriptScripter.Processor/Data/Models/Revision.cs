using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Models
{
    /// <summary>
    /// revision is a script that has already been applied to the database
    /// </summary>
    public class Revision
    {
        public Guid ScriptId { get; set; }
        public string SqlStatement { get; set; }
        public string ScriptDeveloperName { get; set; }
        public string ScriptNotes { get; set; }
        public DateTimeOffset ScriptDate { get; set; }
        public string RunByDeveloperName { get; set; }
        public string RunOnMachineName { get; set; }
        public DateTimeOffset RunDate { get; set; }

        /// <summary>
        /// unique key identifying the revision
        /// </summary>
        //public int RevisionId { get; set; }
    }
}
