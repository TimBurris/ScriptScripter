using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.Contracts
{
    public interface IThemeService
    {
        IEnumerable<Themes.ThemeOption> GetAllThemes();
        void ApplyTheme(string name);
        Themes.ThemeOption GetCurrentTheme();
    }
}
