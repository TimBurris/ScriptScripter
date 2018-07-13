using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace ScriptScripter.Processor.Data.Repositories
{
    public class RevisionRepository : Contracts.IRevisionRepository
    {
        private const string _tableNameWithSchema = "[ScriptScripter].[Revision]";

        private IDbConnection GetConnection(Data.Models.DatabaseConnectionParameters databaseConnectionParms)
        {
            var connString = databaseConnectionParms.GetConnectionString();
            return new System.Data.SqlClient.SqlConnection(connString);
        }

        public Models.Revision GetLastRevision(Data.Models.DatabaseConnectionParameters databaseConnectionParms)
        {
            using (var connection = this.GetConnection(databaseConnectionParms))
            {
                if (!this.IsUnderControl(connection))
                    return null;
                else
                    return connection.QueryFirstOrDefault<Data.Models.Revision>(
                        sql: $@"SELECT * 
                            FROM {_tableNameWithSchema} 
                            Order By [{nameof(Data.Models.Revision.RevisionNumber)}] DESC");
            }
        }

        public bool IsUnderControl(Data.Models.DatabaseConnectionParameters databaseConnectionParms)
        {
            using (var connection = this.GetConnection(databaseConnectionParms))
            {
                return this.IsUnderControl(connection);
            }
        }

        private bool IsUnderControl(IDbConnection connection)
        {
            var sql = $@"SELECT CASE WHEN EXISTS(SELECT * 
                                                    FROM sys.objects 
                                                    WHERE object_id = OBJECT_ID(N'{_tableNameWithSchema}') AND type in (N'U')
                                                    )
                            THEN 1 ELSE 0 END";
            return connection.ExecuteScalar<int>(sql) == 1;
        }
    }
}
