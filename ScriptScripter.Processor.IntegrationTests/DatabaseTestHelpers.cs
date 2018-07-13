using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.IntegrationTests
{
    public class DatabaseTestHelpers
    {
        public static void CreateTestDB(string databaseName, string connectionString)
        {
            string sql = $"create database [{databaseName}];";
            ExecuteNonQuery(sql, connectionString);
        }

        public static void KillTestDB(string databaseName, string connectionString)
        {
            string sql = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
            ExecuteNonQuery(sql, connectionString);

            sql = $"drop database [{databaseName}];";
            ExecuteNonQuery(sql, connectionString);
        }

        public static bool ExecuteExistsSql(string sql, string connectionString)
        {
            return ExecuteBooleanSql($"SELECT 1 WHERE EXISTS ({sql})", connectionString);
        }

        public static bool ExecuteBooleanSql(string sql, string connectionString)
        {
            using (var con = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, connection: con))
                {
                    var x = cmd.ExecuteScalar();
                    if (x == null)
                        return false;
                    else if (x is bool)
                        return (bool)x;
                    else if (x is int)
                        return ((int)x) != 0;
                    else if (x is string)
                        return bool.Parse(x.ToString());
                    else
                        throw new NotSupportedException($"You need to add support for {x.GetType()} to ExecuteBooleanSql");
                }
            }
        }

        public static void ExecuteNonQuery(string sql, string connectionString)
        {
            using (var con = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, connection: con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
