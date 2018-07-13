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
        /// <summary>
        /// RevisionNumber of Script
        /// </summary>
        public int RevisionNumber { get; set; }
        public string SQLStatement { get; set; }
        public string ScriptDeveloperName { get; set; }
        public string ScriptNotes { get; set; }
        public DateTime ScriptDate { get; set; }
        public string RunByDeveloperName { get; set; }
        public string RunOnMachineName { get; set; }
        public DateTime RunDate { get; set; }

        /// <summary>
        /// unique key identifying the revision
        /// </summary>
        public int RevisionId { get; set; }
    }
}
