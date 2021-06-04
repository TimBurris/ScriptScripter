using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services
{
    public class ConfigurationFileUpgradeService : Data.Repositories.ConfigFileBase, Contracts.IConfigurationFileUpgradeService
    {
        public ConfigurationFileUpgradeService(System.IO.Abstractions.IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public void UpgradeFile()
        {
            var settings = this.ReadFile();
            bool hasChanges = false;

            //Step 1 assign container Guids for files that are before we had ContainUid
            hasChanges = this.AssignGuids(settings.ScriptContainers) ? true : hasChanges;


            //************ future migrations go here  **********

            //final step, re-persist all the containers
            if (hasChanges)
                this.WriteFile(settings);
        }

        /// <summary>
        /// returns true if changes were made
        /// </summary>
        /// <param name="scriptContainers"></param>
        /// <returns></returns>
        private bool AssignGuids(IEnumerable<Data.Models.ScriptContainer> scriptContainers)
        {
            if (scriptContainers == null)
                return false;

            bool hasChanges = false;

            foreach (var c in scriptContainers)
            {
                if (c.ContainerUid.Equals(Guid.Empty))
                {
                    c.ContainerUid = Guid.NewGuid();
                    hasChanges = true;
                }
            }

            return hasChanges;
        }
    }
}
