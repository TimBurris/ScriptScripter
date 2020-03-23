using CommandLine;
using System;
namespace ScriptScripter.Command
{
    partial class Program
    {
        public class IncomingOptions
        {
            //[Option('c', "connectionstring", Required = true, HelpText = "The connection string for the database where scripts should be applied")]
            //public string ConnectionString { get; set; }
            [Option('f', "scriptfilepath", Required = true, HelpText = "The full path to the script file that is to be applied")]
            public string ScriptFilePath { get; set; }

            [Option('s', "server", Required = true, HelpText = "The server name (with instance) to connect to")]
            public String Server { get; set; }

            [Option('d', "databasename", Required = true, HelpText = "The database to apply the scripts to")]
            public String DatabaseName { get; set; }


            [Option('t', "trustedconnection", Required = false, HelpText = "y|yes|t|true to use windows auth")]
            public bool UseTrustedConnection { get; set; }

            [Option('p', "password", Required = false, HelpText = "Optional, passord for the database connection use when not using Trusted Connection")]
            public String Password { get; set; }

            [Option('u', "user", Required = false, HelpText = "Optional, passord for the database connection use when not using Trusted Connection")]
            public String Username { get; set; }


            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
        }
    }
}
