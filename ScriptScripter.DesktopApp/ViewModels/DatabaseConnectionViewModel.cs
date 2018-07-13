using NinjaMvvm;
using NinjaMvvm.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class DatabaseConnectionViewModel : ScriptScripterViewModelBase
    {
        private Processor.Data.Models.ServerConnectionParameters _serverConnectionParameters;

        [Ninject.Inject]
        public Processor.Data.Contracts.IConfigurationRepository ConfigurationRepository { get; set; }

        [Ninject.Inject]
        public Processor.Services.Contracts.IScriptingService ScriptingService { get; set; }

        [Ninject.Inject]
        public DatabaseConnectionControlViewModel DatabaseConnectionControlVM { get; set; }

        public DatabaseConnectionViewModel()
        {
            ViewTitle = "Database Connection";
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            _serverConnectionParameters = ConfigurationRepository.GetServerConnectionParameters();

            this.DatabaseConnectionControlVM.Init(_serverConnectionParameters);

            return true;
        }

        #region Connect Command

        private RelayCommand _connectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                    _connectCommand = new RelayCommand((param) => this.ConnectAsync(), (param) => this.CanConnect());
                return _connectCommand;
            }
        }

        public bool CanConnect()
        {
            return true;
        }

        /// <summary>
        /// Executes the Connect command 
        /// </summary>
        public async Task ConnectAsync()
        {
            var p = this.DatabaseConnectionControlVM.BuildConnectionParameters();

            var result = await ScriptingService.TestServerConnectionAsync(p);

            if (!result.WasSuccessful)
            {
                Navigator.ShowDialog<MessageBoxViewModel>(vm =>
                   vm.Init(title: "Test Connection Failed",
                      message: result.Message,
                      buttons: MessageBoxViewModel.MessageBoxButton.OK,
                      icon: MessageBoxViewModel.MessageBoxImage.Error
                      ));

                return;
            }

            ConfigurationRepository.SetServerConnectionParameters(p);

            Navigator.CloseDialog(this);
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
            Navigator.CloseDialog(this);
        }

        #endregion

    }
}