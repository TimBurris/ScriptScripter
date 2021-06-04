using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor
{
    public interface IFileSystemWatcherFactory
    {
        FileSystemWatcher Create();
    }
}
