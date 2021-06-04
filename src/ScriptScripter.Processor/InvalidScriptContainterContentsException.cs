using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor
{
    public class InvalidScriptContainterContentsException : Exception
    {
        public InvalidScriptContainterContentsException()
        {

        }
        public InvalidScriptContainterContentsException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
