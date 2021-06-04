using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Models
{
    public class ServerConnectionParameters
    {
        public bool UseTrustedConnection { get; set; }
        public String Server { get; set; }

        public String Password { get; set; }
        public String Username { get; set; }

        public virtual string GetConnectionString()
        {
            return this.GetConnectionString(databaseName: null);
        }
        public override string ToString()
        {
            string result;

            result = this.Server;

            if (this.UseTrustedConnection)
                result += " (Integrated Security)";
            else
                result += $" ({this.Username} ********)";

            return result;
        }

        protected string GetConnectionString(string databaseName = "master")
        {
            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder["Data Source"] = this.Server;

            if (this.UseTrustedConnection)
                builder["integrated Security"] = true;
            else
            {
                builder["user id"] = this.Username;
                builder["password"] = this.Password;
            }

            builder["Initial Catalog"] = databaseName;

            return builder.ConnectionString;
        }
    }
}
