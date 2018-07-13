using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Contracts
{
    public interface IScriptContainerRepository
    {
        IEnumerable<Models.ScriptContainer> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="scriptFilePath"></param>
        /// <param name="customConnectionParameters">Optionally override the server connection params</param>
        /// <returns></returns>
        Dto.ActionResult AddNew(string databaseName, string scriptFilePath, Models.ServerConnectionParameters customConnectionParameters, IEnumerable<string> tags);

        Dto.ActionResult Update(Models.ScriptContainer scriptContainer);

        Dto.ActionResult Remove(Guid containerUid);

        Models.ScriptContainer GetByUid(Guid containerUid);

    }
}
