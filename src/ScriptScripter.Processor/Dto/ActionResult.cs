using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Dto
{
    public class ActionResult
    {
        public bool WasSuccessful { get; set; }
        public string Message { get; set; }

        public static ActionResult SuccessResult()
        {
            return new Dto.ActionResult() { WasSuccessful = true };
        }
        public static ActionResult SuccessResult<T>(T result)
        {
            return new Dto.ActionResult<T>() { WasSuccessful = true, Result = result };
        }
        public static ActionResult FailedResult(string message)
        {
            return new Dto.ActionResult() { Message = message };
        }
    }

    public class ActionResult<T> : ActionResult
    {
        public T Result { get; set; }

        public static ActionResult<T> SuccessResult(T result)
        {
            return new Dto.ActionResult<T>() { Result = result, WasSuccessful = true };
        }

        new public static ActionResult<T> FailedResult(string message)
        {
            return new Dto.ActionResult<T>() { Message = message };
        }
    }
}
