using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.Contracts
{
    public interface IFileNameDropHandler
    {

        void Init(System.Windows.UIElement element);

        Action<string> OnDropAction { get; set; }
    }
}
