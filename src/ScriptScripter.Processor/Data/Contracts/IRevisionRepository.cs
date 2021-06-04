using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Contracts
{
    public interface IRevisionRepository
    {
        IEnumerable<Models.Revision> GetAll(Data.Models.DatabaseConnectionParameters databaseConnectionParms);
        Models.Revision GetLastRevision(Data.Models.DatabaseConnectionParameters database);

        /// <summary>
        /// returns true if the script scripter revision table exists in the database, else returns false
        /// </summary>
        /// <param name="databaseConnectionParms"></param>
        /// <returns></returns>
        bool IsUnderControl(Data.Models.DatabaseConnectionParameters databaseConnectionParms);

        //Future?  Models.Revision GetRevisionByRevisionId(string databaseName, int revisionId);

        //revisions are added as part of the scripting process, not this repo
        //Dto.ActionResult AddNewRevision(string databaseName, Models.Revision revision);

    }
}
