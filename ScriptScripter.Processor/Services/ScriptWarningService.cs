using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services
{
    public class ScriptWarningService : Contracts.IScriptWarningService
    {
        public IEnumerable<string> CheckSql(string sql)
        {
            var lst = new List<string>();
            if (string.IsNullOrEmpty(sql))
                return lst;

            var lines = sql.Split(new string[] { "\r\n" }, StringSplitOptions.None);//don't remove empty because that would throw off our line numbers
            int i = 0;
            foreach (var line in lines)
            {
                i++;
                if (line.TrimStart().StartsWith("use ", StringComparison.OrdinalIgnoreCase))
                {
                    lst.Add($"Line {i}: Appears to switch databases with a 'Use [db]' statement; typically you should let ScriptScripter control the database.");
                }
            }

            return lst;
        }
    }
}
