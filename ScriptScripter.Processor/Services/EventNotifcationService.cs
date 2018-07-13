using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services
{
    public class EventNotifcationService : Contracts.IEventNotificationService
    {
        public event EventHandler ServerConnectionChanged;
        public event EventHandler<EventArgs<Data.Models.ScriptContainer>> ScriptContainerAdded;
        public event EventHandler<EventArgs<Data.Models.ScriptContainer>> ScriptContainerUpdated;
        public event EventHandler<EventArgs<Data.Models.ScriptContainer>> ScriptContainerRemoved;
        public event EventHandler<EventArgs<Data.Models.ScriptContainer>> ScriptContainerContentsChanged;
        public void NotifyServerConnectionChanged()
        {
            ServerConnectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyScriptContainerAdded(Data.Models.ScriptContainer scriptContainer)
        {
            ScriptContainerAdded?.Invoke(this, new Processor.EventArgs<Data.Models.ScriptContainer>(scriptContainer));
        }
        public void NotifyScriptContainerUpdated(Data.Models.ScriptContainer scriptContainer)
        {
            ScriptContainerUpdated?.Invoke(this, new Processor.EventArgs<Data.Models.ScriptContainer>(scriptContainer));
        }

        public void NotifyScriptContainerRemoved(Data.Models.ScriptContainer scriptContainer)
        {
            ScriptContainerRemoved?.Invoke(this, new Processor.EventArgs<Data.Models.ScriptContainer>(scriptContainer));
        }

        public void NotifyScriptContainerContentsChanged(Data.Models.ScriptContainer scriptContainer)
        {
            ScriptContainerContentsChanged?.Invoke(this, new Processor.EventArgs<Data.Models.ScriptContainer>(scriptContainer));
        }
    }
}
