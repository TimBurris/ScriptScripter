using CommandLine;

namespace ScriptScripter.DesktopApp
{
    public class IncomingOptions
    {
        [Option('a', "addscript", Required = false, HelpText = "The full path to the script file or folder to initiate an 'Add New Script'")]
        public string AddScriptContainerPath { get; set; }


        [Option('c', "clipboard", Required = false, Default = false, HelpText = "When combined with -a or --addscript, use the clipboard for the new script content")]
        public bool UseClipboardForNewScript { get; set; }

    }
}
