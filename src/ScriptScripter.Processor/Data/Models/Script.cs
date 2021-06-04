using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Models
{
    /// <summary>
    /// A scripte is a sql script that comes from the repository of scripts (a script file)
    /// </summary>
    public class Script
    {
        public Guid ScriptId { get; set; }
        public string SqlStatement { get; set; }
        public string DeveloperName { get; set; }
        public string Notes { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public DateTimeOffset ScriptDate { get; set; }

        /// <summary>
        /// this property exists only as a hack to solve xml serializer's inability to serialize datetimeoffset
        /// </summary>
        [System.Xml.Serialization.XmlElement("ScriptDate")]
        public string XScriptDateForXml // format: 2011-11-11T15:05:46.4733406+01:00
        {
            get { return ScriptDate.ToString("o"); } // o = yyyy-MM-ddTHH:mm:ss.fffffffzzz
            set { ScriptDate = DateTimeOffset.Parse(value); }
        }
    }
}
