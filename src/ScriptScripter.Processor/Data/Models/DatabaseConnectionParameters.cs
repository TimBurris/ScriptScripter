using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Models
{
    //the reason we need this, rather than just a simple connection string, is that when we get down to running this in SMO, we need the individual components
    public class DatabaseConnectionParameters : ServerConnectionParameters
    {
        public DatabaseConnectionParameters()
        {

        }
        public DatabaseConnectionParameters(ServerConnectionParameters copyFrom)
        {
            this.UseTrustedConnection = copyFrom.UseTrustedConnection;
            this.Server = copyFrom.Server;
            this.Username = copyFrom.Username;
            this.Password = copyFrom.Password;
        }
        public String DatabaseName { get; set; }

        public override string GetConnectionString()
        {
            return base.GetConnectionString(this.DatabaseName);
        }
    }
}
