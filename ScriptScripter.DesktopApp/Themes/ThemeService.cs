using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.Themes
{
    public class ThemeService : Contracts.IThemeService
    {
        private const string _defaultThemeName = "Steel";
        private const string _themeBrushResourceName = "AccentColorBrush";

        public IEnumerable<ThemeOption> GetAllThemes()
        {
            //this code mostly came from the MahApps published sample/example project
            return MahApps.Metro.ThemeManager.Accents
                .Select(a => new ThemeOption()
                {
                    Name = a.Name,
                    ColorBrush = a.Resources[_themeBrushResourceName] as System.Windows.Media.Brush
                }
                )
            .ToList();
        }

        public ThemeOption GetCurrentTheme()
        {
            var app = System.Windows.Application.Current;

            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(app);
            var accent = MahApps.Metro.ThemeManager.GetAccent(theme.Item1.Name);

            return new ThemeOption()
            {
                Name = accent.Name,
                ColorBrush = accent.Resources[_themeBrushResourceName] as System.Windows.Media.Brush
            };
        }

        public void ApplyTheme(string name)
        {
            //this code mostly came from the MahApps published sample/example project
            var app = System.Windows.Application.Current;

            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(app);
            var accent = MahApps.Metro.ThemeManager.GetAccent(name);

            if (accent == null)
                accent = MahApps.Metro.ThemeManager.GetAccent(_defaultThemeName);

            MahApps.Metro.ThemeManager.ChangeAppStyle(app, accent, theme.Item1);
        }
    }
}
