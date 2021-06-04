using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class ErrorViewModel : MessageBoxViewModel
    {
        //public ErrorViewModel() { }//designer only   //removed because for somereason IoC is using this ctor instead of the correct one
        public ErrorViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator, NLog.ILogger logger)
            : base(navigator, logger)
        {

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
