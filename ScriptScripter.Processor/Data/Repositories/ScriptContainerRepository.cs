using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Repositories
{
    public class ScriptContainerRepository : ConfigFileBase, Contracts.IScriptContainerRepository
    {
        [Ninject.Inject]
        public Services.Contracts.IEventNotificationService EventNotificationService { get; set; }

        public ScriptContainerRepository(System.IO.Abstractions.IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public ScriptContainerRepository(System.IO.Abstractions.IFileSystem fileSystem, string configurationFileName)
            : base(fileSystem, configurationFileName)
        {
        }

        public ScriptContainerRepository()
            : base()
        {
        }

        public IEnumerable<Models.ScriptContainer> GetAll()
        {
            var settings = this.ReadFile();

            return settings.ScriptContainers ?? new List<Models.ScriptContainer>();
        }

        public Dto.ActionResult AddNew(string databaseName, string scriptFilePath,
            Models.ServerConnectionParameters customConnectionParameters, IEnumerable<string> tags)
        {
            var newContainer = new Models.ScriptContainer()
            {
                ContainerUid = Guid.NewGuid(),
                DatabaseName = databaseName,
                ScriptFilePath = scriptFilePath,
                CustomServerConnectionParameters = customConnectionParameters,
                Tags = tags?.ToList()
            };

            var settings = this.ReadFile();
            var containers = settings.ScriptContainers?.ToList() ?? new List<Models.ScriptContainer>();
            var container = this.FindMatch(containers, newContainer);

            if (container == null)
            {
                containers.Add(newContainer);
                settings.ScriptContainers = containers;

                this.WriteFile(settings);

                this.EventNotificationService.NotifyScriptContainerAdded(newContainer);

                return Dto.ActionResult.SuccessResult();
            }
            else
            {
                return new Dto.ActionResult() { Message = "Database already in the list" };
            }
        }


        public Dto.ActionResult Update(Models.ScriptContainer scriptContainer)
        {
            var settings = this.ReadFile();
            var containers = settings.ScriptContainers?.ToList() ?? new List<Models.ScriptContainer>();
            var existingContainer = containers.SingleOrDefault(c => c.ContainerUid == scriptContainer.ContainerUid);

            if (existingContainer == null)
            {
                return new Dto.ActionResult() { Message = "Database not in the list" };
            }
            else
            {
                containers.Remove(existingContainer);
                containers.Add(scriptContainer);
                settings.ScriptContainers = containers;

                this.WriteFile(settings);

                this.EventNotificationService.NotifyScriptContainerUpdated(scriptContainer);

                return Dto.ActionResult.SuccessResult();
            }
        }

        public Dto.ActionResult Remove(Guid containerUid)
        {
            var settings = this.ReadFile();
            var containers = settings.ScriptContainers.ToList();

            var container = containers.SingleOrDefault(c => c.ContainerUid == containerUid);

            if (container == null)
                return new Dto.ActionResult() { Message = "Database not in the list" };
            else
            {
                settings.ScriptContainers.Remove(container);

                this.WriteFile(settings);

                this.EventNotificationService.NotifyScriptContainerRemoved(container);

                return Dto.ActionResult.SuccessResult();
            }
        }

        public Models.ScriptContainer GetByUid(Guid containerUid)
        {
            var settings = this.ReadFile();
            var containers = settings.ScriptContainers?.ToList() ?? new List<Models.ScriptContainer>();

            return containers.SingleOrDefault(c => c.ContainerUid == containerUid);
        }

        private Models.ScriptContainer FindMatch(IEnumerable<Models.ScriptContainer> containers, Models.ScriptContainer script)
        {
            return containers.SingleOrDefault(c => c.DatabaseName.Equals(script.DatabaseName, StringComparison.InvariantCultureIgnoreCase)
                                    && string.Equals(script?.CustomServerConnectionParameters?.Server, c?.CustomServerConnectionParameters?.Server, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
