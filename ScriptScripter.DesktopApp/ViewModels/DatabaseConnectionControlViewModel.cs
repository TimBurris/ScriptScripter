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
    public class DatabaseConnectionControlViewModel : ScriptScripterViewModelBase
    {
        private System.Threading.CancellationTokenSource _testConnectionCancellationTokenSource;
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Processor.Services.Contracts.IScriptingService _scriptingService;

     //   public DatabaseConnectionControlViewModel() { }//designer only  //removed because for somereason IoC is using this ctor instead of the correct one

        public DatabaseConnectionControlViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Processor.Services.Contracts.IScriptingService scriptingService)
        {
            this._navigator = navigator;
            this._scriptingService = scriptingService;
        }

        public Processor.Data.Models.ServerConnectionParameters BuildConnectionParameters()
        {
            var p = new Processor.Data.Models.ServerConnectionParameters();
            p.Username = Username;
            p.Password = Password;
            p.Server = ServerName;
            p.UseTrustedConnection = UseTrustedConnection;

            return p;
        }

        public void Init(Processor.Data.Models.ServerConnectionParameters serverConnectionParameters)
        {
            this.Username = serverConnectionParameters.Username;
            this.Password = serverConnectionParameters.Password;
            this.ServerName = serverConnectionParameters.Server;
            this.UseTrustedConnection = serverConnectionParameters.UseTrustedConnection;
        }

        #region Binding props and Lists

        public bool IsRunningTestConnection
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string ServerName
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool UseTrustedConnection
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string Username
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string Password
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        #endregion

        #region TestConnection Command

        private RelayCommand _testConnectionCommand;
        public RelayCommand TestConnectionCommand
        {
            get
            {
                if (_testConnectionCommand == null)
                    _testConnectionCommand = new RelayCommand((param) => this.TestConnectionAsync(), (param) => this.CanTestConnection());
                return _testConnectionCommand;
            }
        }

        public bool CanTestConnection()
        {
            return true;
        }
        /// <summary>
        /// Executes the TestConnection command 
        /// </summary>
        public async void TestConnectionAsync()
        {
            _testConnectionCancellationTokenSource = new System.Threading.CancellationTokenSource();
            this.IsRunningTestConnection = true;
            try
            {
                var p = BuildConnectionParameters();
                var result = await _scriptingService.TestServerConnectionAsync(p, _testConnectionCancellationTokenSource.Token);

                if (result.WasSuccessful)
                {
                    _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                        vm.Init(title: "Connection Successful",
                            message: "Test connection succeeded!",
                            buttons: MessageBoxViewModel.MessageBoxButton.OK,
                            icon: MessageBoxViewModel.MessageBoxImage.Information
                            )
                        );
                }
                else
                {
                    _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                       vm.Init(title: "Connection Failed",
                           message: result.Message,
                           buttons: MessageBoxViewModel.MessageBoxButton.OK,
                           icon: MessageBoxViewModel.MessageBoxImage.Error
                           )
                       );
                }
            }
            finally
            {
                this.IsRunningTestConnection = false;
                _testConnectionCancellationTokenSource = null;
            }
        }

        #endregion

        #region CancelTestConnection Command

        private RelayCommand _cancelTestConnectionCommand;

        public RelayCommand CancelTestConnectionCommand
        {
            get
            {
                if (_cancelTestConnectionCommand == null)
                    _cancelTestConnectionCommand = new RelayCommand((param) => this.CancelTestConnection(), (param) => this.CanCancelTestConnection());
                return _cancelTestConnectionCommand;
            }
        }

        public bool CanCancelTestConnection()
        {
            return true;
        }

        /// <summary>
        /// Executes the CancelTestConnection command 
        /// </summary>
        public void CancelTestConnection()
        {
            _testConnectionCancellationTokenSource.Cancel();
        }
        #endregion

        #region Designer data
        protected override void OnLoadDesignData()
        {
            ServerName = "(local)\\MyInstance";
            Username = "DumpsterNinja";
            Password = "pizza";
        }

        #endregion
    }
}