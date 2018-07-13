using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services.Contracts
{

    public interface IEventNotificationService
    {
        /// <summary>
        /// fired when the ServerConnection, in Configuration, has changed
        /// </summary>
        event EventHandler ServerConnectionChanged;
        /// <summary>
        /// fired when a ScriptContainer has been Added
        /// </summary>
        event EventHandler<EventArgs<Data.Models.ScriptContainer>> ScriptContainerAdded;
        /// <summary>
        /// fired when a ScriptContainer has been Changed properties have changed
        /// </summary>
        /// <remarks>
        /// This is not that it's contents have changed, that's a different event, this is that the db name, tags, location or something about the container changed
        /// </remarks>
        event EventHandler<EventArgs<Data.Models.ScriptContainer>> ScriptContainerUpdated;
        /// <summary>
        /// fired when a ScriptContainer has been removed
        /// </summary>
        event EventHandler<EventArgs<Data.Models.ScriptContainer>> ScriptContainerRemoved;
        /// <summary>
        /// fired when the "Contents" of the Script Container has changed (e.g. the underlying file is replaced)
        /// </summary>
        event EventHandler<EventArgs<Data.Models.ScriptContainer>> ScriptContainerContentsChanged;


        void NotifyServerConnectionChanged();
        void NotifyScriptContainerAdded(Data.Models.ScriptContainer scriptContainer);
        void NotifyScriptContainerUpdated(Data.Models.ScriptContainer scriptContainer);
        void NotifyScriptContainerRemoved(Data.Models.ScriptContainer scriptContainer);
        void NotifyScriptContainerContentsChanged(Data.Models.ScriptContainer scriptContainer);
    }
}
