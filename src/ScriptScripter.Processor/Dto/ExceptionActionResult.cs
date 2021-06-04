using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Dto
{
    /// <summary>
    /// a derrivation of ActionResult that includes the underlying System.Exception
    /// </summary>
    public class ExceptionActionResult : ExceptionActionResult<object>
    {
        new public static ExceptionActionResult SuccessResult()
        {
            return new Dto.ExceptionActionResult() { WasSuccessful = true };
        }

        new public static ExceptionActionResult FailedResult(Exception exception)
        {
            return new ExceptionActionResult()
            {
                Exception = exception,
                Message = exception.Message
            };
        }
    }

    /// <summary>
    /// a derrivation of ActionResult that includes the underlying System.Exception
    /// </summary>
    public class ExceptionActionResult<T> : ActionResult<T>
    {
        public Exception Exception { get; set; }

        public static ExceptionActionResult<T> FailedResult(Exception exception)
        {
            return new ExceptionActionResult<T>()
            {
                Exception = exception,
                Message = exception.Message
            };
        }

        new public static ExceptionActionResult<T> SuccessResult(T result)
        {
            return new Dto.ExceptionActionResult<T>() { Result = result, WasSuccessful = true };
        }
    }
}
