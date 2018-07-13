using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Models
{
    public class Script
    {
        public int RevisionNumber { get; set; }
        public string SQLStatement { get; set; }
        public string DeveloperName { get; set; }
        public string Notes { get; set; }
        public DateTime ScriptDate { get; set; }
    }
}
