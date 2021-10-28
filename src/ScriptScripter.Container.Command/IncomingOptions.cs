using CommandLine;
using System;
namespace ScriptScripter.Container.Command
{
    public class IncomingOptions
    {
        [Option('s', "sourcecontainerpath", Required = true, HelpText = "The full path to the script file or folder to copy scripts from")]
        public string SourceContainerPath { get; set; }

        [Option('d', "destinationcontainerpath", Required = true, HelpText = "The full path to the script file or folder to copy scripts to")]
        public string DestinationContainerPath { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
