using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs()
        {

        }

        public EventArgs(T eventData)
        {
            this.EventData = eventData;
        }

        public T EventData { get; set; }
    }
}
