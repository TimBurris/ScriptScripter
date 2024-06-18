using CommandLine;

namespace ScriptScripter.DesktopApp
{
    public class IncomingOptions
    {
        [Option('a', "addscript", Required = false, HelpText = "The full path to the script file or folder to initiate an 'Add New Script'")]
        public string AddScriptContainerPath { get; set; }

    }
}
