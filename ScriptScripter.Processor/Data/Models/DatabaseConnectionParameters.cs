using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Models
{
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
