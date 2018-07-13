using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services
{
    public class ExecuteFailedEventArgs : EventArgs
    {
        public ExecuteFailedEventArgs() { }
        public ExecuteFailedEventArgs(Exception exceptionRecieved)
        {
            this.ExceptionRecieved = exceptionRecieved;
        }
        /// <summary>
        /// specifies whether or not the raising object should retry executing
        /// </summary>
        public bool ShouldRetry { get; set; }

        /// <summary>
        /// A reference to the exception that caused Execute to fail
        /// </summary>
        public Exception ExceptionRecieved { get; set; }
    }
}
