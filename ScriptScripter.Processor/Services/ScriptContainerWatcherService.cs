using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptScripter.Processor.Data.Models;

namespace ScriptScripter.Processor.Services
{
    public class ScriptContainerWatcherService : Contracts.IScriptContainerWatcherService, IDisposable
    {
        private class WatcherContainerPair
        {
            public WatcherContainerPair() { }
            public WatcherContainerPair(System.IO.FileSystemWatcher watcher, ScriptContainer container)
            {
                this.Watcher = watcher;
                this.Container = container;
            }

            public System.IO.FileSystemWatcher Watcher { get; set; }
            public ScriptContainer Container { get; set; }
        }

        private Data.Contracts.IScriptContainerRepository _scriptContainerRepository;
        private Services.Contracts.IEventNotificationService _eventNotificationService;
        private IFileSystemWatcherFactory _fileSystemWatcherFactory;

        private Dictionary<string, WatcherContainerPair> _watchers = new Dictionary<string, WatcherContainerPair>();

        public ScriptContainerWatcherService(Data.Contracts.IScriptContainerRepository scriptContainerRepository, Services.Contracts.IEventNotificationService eventNotificationService,
            IFileSystemWatcherFactory fileSystemWatcherFactory)
        {
            _scriptContainerRepository = scriptContainerRepository;
            _eventNotificationService = eventNotificationService;
            _fileSystemWatcherFactory = fileSystemWatcherFactory;
        }

        #region IScriptContainerWatcherService implementation

        public void BeginWatchingAllContainers()
        {
            _eventNotificationService.ScriptContainerAdded += EventNotificationService_ScriptContainerAdded;
            _eventNotificationService.ScriptContainerUpdated += _eventNotificationService_ScriptContainerUpdated;
            _eventNotificationService.ScriptContainerRemoved += EventNotificationService_ScriptContainerRemoved;

            foreach (var container in _scriptContainerRepository.GetAll())
            {
                this.AddWatcherForContainer(container);
            }
        }

        private void _eventNotificationService_ScriptContainerUpdated(object sender, EventArgs<ScriptContainer> e)
        {
            //remove and readd because the script location might have changed
            this.RemoveWatcherForContainer(e.EventData);
            this.AddWatcherForContainer(e.EventData);
        }

        private void EventNotificationService_ScriptContainerRemoved(object sender, EventArgs<Data.Models.ScriptContainer> e)
        {
            this.RemoveWatcherForContainer(e.EventData);
        }

        private void EventNotificationService_ScriptContainerAdded(object sender, EventArgs<Data.Models.ScriptContainer> e)
        {
            this.AddWatcherForContainer(e.EventData);
        }

        public void EndWatchingAllcontainers()
        {
            _eventNotificationService.ScriptContainerAdded -= EventNotificationService_ScriptContainerAdded;
            _eventNotificationService.ScriptContainerUpdated -= _eventNotificationService_ScriptContainerUpdated;
            _eventNotificationService.ScriptContainerRemoved -= EventNotificationService_ScriptContainerRemoved;

            foreach (var key in _watchers.Keys.ToList())
            {
                this.RemoveWatcher(key);
            }
        }

        #endregion

        private void ContainerFile_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            var w = sender as System.IO.FileSystemWatcher;
            if (w == null)
            {
                //TODO: log as an error or warning or something
                return;
            }

            var container = _watchers.Values.SingleOrDefault(p => p.Watcher == w)?.Container;

            if (container == null)
            {
                //TODO: log as an error or warning or something
                return;
            }
            _eventNotificationService.NotifyScriptContainerContentsChanged(container);
        }

        private string GetContainerKey(ScriptContainer container)
        {
            var fle = new System.IO.FileInfo(container.ContainerUid.ToString());

            return fle.FullName.ToLower();
        }

        private void AddWatcherForContainer(ScriptContainer container)
        {
            var key = GetContainerKey(container);
            if (_watchers.ContainsKey(key))
                return;

            //TODO: change this to abstractions so that tests don't require actual folder to exist

            //you can't watch a folder that does not exist, so just bail
            var fle = new System.IO.FileInfo(container.ScriptFilePath);
            if (!fle.Directory.Exists)
                return;

            var w = _fileSystemWatcherFactory.Create();
            w.Path = fle.DirectoryName;
            w.Filter = fle.Name;

            _watchers.Add(key, new WatcherContainerPair(w, container));

            w.Changed += ContainerFile_Changed;
            w.Created += ContainerFile_Changed;
            w.Deleted += ContainerFile_Changed;

            // Begin watching.
            w.EnableRaisingEvents = true;
        }

        private void RemoveWatcherForContainer(ScriptContainer container)
        {
            var key = GetContainerKey(container);
            this.RemoveWatcher(key);
        }

        private void RemoveWatcher(string key)
        {
            if (_watchers.ContainsKey(key))
            {
                var w = _watchers[key].Watcher;
                w.EnableRaisingEvents = false;

                w.Changed -= ContainerFile_Changed;
                w.Created -= ContainerFile_Changed;
                w.Deleted -= ContainerFile_Changed;

                w.Dispose();
                _watchers.Remove(key);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.EndWatchingAllcontainers();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
