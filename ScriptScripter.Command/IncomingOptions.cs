using CommandLine;
using System;
namespace ScriptScripter.Command
{
    public class IncomingOptions
    {
        [Option('f', "scriptfilepath", Required = true, HelpText = "The full path to the script file that is to be applied")]
        public string ScriptFilePath { get; set; }

        [Option('s', "server", Required = true, HelpText = "The server name (with instance) to connect to")]
        public String Server { get; set; }

        [Option('d', "databasename", Required = true, HelpText = "The database to apply the scripts to")]
        public String DatabaseName { get; set; }

        [Option('t', "trustedconnection", Required = false, HelpText = "Optional, when specifed, connect to the database using Windows Authentication")]
        public bool UseTrustedConnection { get; set; }

        [Option('u', "user", Required = false, HelpText = "Optional, user for the database connection use when not using Trusted Connection")]
        public String Username { get; set; }

        [Option('p', "password", Required = false, HelpText = "Optional, password for the database connection use when not using Trusted Connection")]
        public String Password { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
