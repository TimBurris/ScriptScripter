using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaMvvm;
using NinjaMvvm.Wpf;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class RetryActionViewModel : ScriptScripterViewModelBase
    {
        public RetryActionViewModel()
        {
            ViewTitle = "Failed to execute";
        }

        protected override void OnLoadDesignData()
        {
            this.ViewTitle = "Lorem ipsum";
            this.Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco";
        }

        public string Message
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool ViewResult
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        #region Retry Command

        private RelayCommand _retryCommand;
        public RelayCommand RetryCommand
        {
            get
            {
                if (_retryCommand == null)
                    _retryCommand = new RelayCommand((param) => this.Retry(), (param) => this.CanRetry());
                return _retryCommand;
            }
        }

        public bool CanRetry()
        {
            return true;
        }

        /// <summary>
        /// Executes the Retry command 
        /// </summary>
        public void Retry()
        {
            this.ViewResult = true;
            this.Navigator.CloseDialog(this);
        }

        #endregion

        #region Cancel Command

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new RelayCommand((param) => this.Cancel(), (param) => this.CanCancel());
                return _cancelCommand;
            }
        }

        public bool CanCancel()
        {
            return true;
        }

        /// <summary>
        /// Executes the Cancel command 
        /// </summary>
        public void Cancel()
        {
            this.ViewResult = false;
            this.Navigator.CloseDialog(this);
        }

        #endregion

    }
}
