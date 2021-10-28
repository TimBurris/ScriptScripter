using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Models
{
    public class ScriptContainer
    {
        public Guid ContainerUid { get; set; }
        public string DatabaseName { get; set; }
        /// <summary>
        /// file or folder where all the scripts are held
        /// </summary>
        public string ScriptContainerPath { get; set; }
        public ServerConnectionParameters CustomServerConnectionParameters { get; set; }
        public List<string> Tags { get; set; }
    }
}
