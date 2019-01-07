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
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;
        private readonly Processor.Services.Contracts.IScriptingService _scriptingService;
        private readonly DatabaseConnectionControlViewModel _databaseConnectionControlVM;


        public DatabaseConnectionViewModel() { }//designer only
        public DatabaseConnectionViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository,
            Processor.Services.Contracts.IScriptingService scriptingService,
             DatabaseConnectionControlViewModel databaseConnectionControlVM)
        {
            ViewTitle = "Database Connection";
            this._navigator = navigator;
            this._configurationRepository = configurationRepository;
            this._scriptingService = scriptingService;
            this._databaseConnectionControlVM = databaseConnectionControlVM;
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            _serverConnectionParameters = _configurationRepository.GetServerConnectionParameters();

            _databaseConnectionControlVM.Init(_serverConnectionParameters);

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
            var p = _databaseConnectionControlVM.BuildConnectionParameters();

            var result = await _scriptingService.TestServerConnectionAsync(p);

            if (!result.WasSuccessful)
            {
                _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                   vm.Init(title: "Test Connection Failed",
                      message: result.Message,
                      buttons: MessageBoxViewModel.MessageBoxButton.OK,
                      icon: MessageBoxViewModel.MessageBoxImage.Error
                      ));

                return;
            }

            _configurationRepository.SetServerConnectionParameters(p);

            _navigator.CloseDialog(this);
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
            _navigator.CloseDialog(this);
        }

        #endregion

    }
}