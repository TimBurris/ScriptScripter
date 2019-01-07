using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class ErrorViewModel : MessageBoxViewModel
    {
        private readonly NLog.ILogger _logger;

        public ErrorViewModel() { }//designer only
        public ErrorViewModel(NLog.ILogger logger)
        {
            this._logger = logger;
        }

        public void LoadFromException(Exception ex)
        {
            _logger.Error(ex, "Error recieved and being shown to user");

            this.ViewTitle = "Unexpected error recieved";
            this.Message = "The following error was recieved (and logged based on your NLog settings in config): " + ex.Message;
            this.MoreDetailsMessage = this.GetDetailedExceptionMessage(ex);
            this.CanShowMoreDetails = true;
            this.MoreDetailsCaption = "Show error info";
            this.Icon = MessageBoxImage.Error;
            this.SetButtons(MessageBoxButton.OK);
        }

        private string GetDetailedExceptionMessage(Exception ex)
        {
            var message = string.Empty;

            var tempEx = ex;

            while (tempEx != null)
            {
                message += tempEx.Message + Environment.NewLine + tempEx.StackTrace + Environment.NewLine + "******************************************" + Environment.NewLine;
                tempEx = tempEx.InnerException;
            }
            return message;
        }
    }
}
