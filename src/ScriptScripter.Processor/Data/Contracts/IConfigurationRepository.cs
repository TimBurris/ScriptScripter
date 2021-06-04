using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Contracts
{
    public interface IConfigurationRepository
    {
        string GetDeveloperName();
        void SetDeveloperName(string developerName);

        Models.ServerConnectionParameters GetServerConnectionParameters();

        void SetServerConnectionParameters(Models.ServerConnectionParameters connectionParameters);
        string GetThemeName();
        void SetThemeName(string themeName);
    }
}
