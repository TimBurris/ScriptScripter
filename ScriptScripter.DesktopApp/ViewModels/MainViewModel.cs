using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NinjaMvvm;
using NinjaMvvm.Wpf;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class MainViewModel : ScriptScripterViewModelBase
    {
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Processor.Services.Contracts.IScriptContainerWatcherService _scriptContainerWatcherService;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private readonly Processor.Services.Contracts.IConfigurationFileUpgradeService _configurationFileUpgradeService;
        private readonly Contracts.IThemeService _themeService;
        private readonly Processor.Services.Contracts.IEventNotificationService _eventNotificationService;

        //public MainViewModel() { }//designer only   //removed because for somereason IoC is using this ctor instead of the correct one

        public MainViewModel(
            NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Processor.Services.Contracts.IScriptContainerWatcherService scriptContainerWatcherService,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Processor.Services.Contracts.IEventNotificationService eventNotificationService,
            Processor.Services.Contracts.IConfigurationFileUpgradeService configurationFileUpgradeService,
            Contracts.IThemeService themeService,
            NLog.ILogger logger)
            : base(logger)
        {
            ViewTitle = "ScriptScripter";
            this._navigator = navigator;
            this._scriptContainerWatcherService = scriptContainerWatcherService;
            this._configurationRepository = configurationRepository;
            this._viewModelFaultlessService = viewModelFaultlessService;
            this._eventNotificationService = eventNotificationService;
            this._configurationFileUpgradeService = configurationFileUpgradeService;
            this._themeService = themeService;
            if (_eventNotificationService != null)
                _eventNotificationService.ServerConnectionChanged += _eventNotificationService_ServerConnectionChanged;
        }


        private void _eventNotificationService_ServerConnectionChanged(object sender, EventArgs e)
        {
            var connectionParams = _configurationRepository.GetServerConnectionParameters();
            BindConnectionInfo(connectionParams);
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            //first things first, upgrad config
            var upgradeResult = await _viewModelFaultlessService
                           .TryExecuteSyncAsAsync(() => _configurationFileUpgradeService.UpgradeFile());

            if (!upgradeResult.WasSuccessful)
                return false;

            var configurationInfo = new ViewModels.MainViewModel.ConfigurationInfo();

            var result = await _viewModelFaultlessService
                .TryExecuteSyncAsAsync(() =>
                {
                    configurationInfo.DeveloperName = _configurationRepository.GetDeveloperName();
                    configurationInfo.ServerConnectionParams = _configurationRepository.GetServerConnectionParameters();
                    configurationInfo.ThemeName = _configurationRepository.GetThemeName();
                    _scriptContainerWatcherService.BeginWatchingAllContainers();
                });

            if (!result.WasSuccessful)
                return false;


            if (!string.IsNullOrEmpty(configurationInfo.ThemeName))
                _themeService.ApplyTheme(name: configurationInfo.ThemeName);

            BindDeveloperName(configurationInfo.DeveloperName);
            BindConnectionInfo(configurationInfo.ServerConnectionParams);
            ReturnHome();

            return true;
        }

        private void BindConnectionInfo(Processor.Data.Models.ServerConnectionParameters connectionInfo)
        {
            if (string.IsNullOrEmpty(connectionInfo.Server))
            {
                ServerName = "- connection not set -";
                Authentication = null;
                return;
            }
            ServerName = connectionInfo.Server;

            if (connectionInfo.UseTrustedConnection)
                Authentication = "Integrated Security";
            else
                Authentication = $"{connectionInfo.Username} ********";
        }

        private void BindDeveloperName(string developerName)
        {
            if (string.IsNullOrEmpty(developerName))
                this.DeveloperName = "- developer name not set -";
            else
                this.DeveloperName = developerName;
        }

        protected override void OnLoadDesignData()
        {
            DeveloperName = "Jack Sparrow";
            ServerName = "(local)\\instance";
            Authentication = "Integrated Security";
        }

        public string DeveloperName
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string ServerName
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string Authentication
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool IsOnHomeView
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        //NOTE: this Prop is One way to Source, we don't change the Model from here, we only have this here so we can know if we are "home" or not
        public ScriptScripterViewModelBase ContentViewModel
        {
            get { return GetField<ScriptScripterViewModelBase>(); }
            set
            {
                if (SetField(value))
                {
                    if (value != null && value is DatabaseListViewModel)
                        IsOnHomeView = true;
                    else
                        IsOnHomeView = false;
                }
            }
        }

        #region ReturnHome Command

        private RelayCommand _returnHomeCommand;
        public RelayCommand ReturnHomeCommand
        {
            get
            {
                if (_returnHomeCommand == null)
                    _returnHomeCommand = new RelayCommand((param) => this.ReturnHome(), (param) => this.CanReturnHome());
                return _returnHomeCommand;
            }
        }

        public bool CanReturnHome()
        {
            return true;
        }

        /// <summary>
        /// Executes the ReturnHome command 
        /// </summary>
        public void ReturnHome()
        {
            _navigator.NavigateTo<DatabaseListViewModel>();
        }

        #endregion

        #region ChangeServer Command

        private RelayCommand _changeServerCommand;
        public RelayCommand ChangeServerCommand
        {
            get
            {
                if (_changeServerCommand == null)
                    _changeServerCommand = new RelayCommand((param) => this.ChangeServer(), (param) => this.CanChangeServer());
                return _changeServerCommand;
            }
        }

        public bool CanChangeServer()
        {
            return true;
        }

        /// <summary>
        /// Executes the ChangeServer command 
        /// </summary>
        public void ChangeServer()
        {
            _navigator.ShowDialog<DatabaseConnectionViewModel>();
        }

        #endregion

        #region ChangeDeveloper Command

        private RelayCommand _changeDeveloperCommand;
        public RelayCommand ChangeDeveloperCommand
        {
            get
            {
                if (_changeDeveloperCommand == null)
                    _changeDeveloperCommand = new RelayCommand((param) => this.ChangeDeveloper(), (param) => this.CanChangeDeveloper());
                return _changeDeveloperCommand;
            }
        }

        public bool CanChangeDeveloper()
        {
            return true;
        }

        /// <summary>
        /// Executes the ChangeDeveloper command 
        /// </summary>
        public void ChangeDeveloper()
        {

            _navigator.ShowDialog<DeveloperNameViewModel>();

            BindDeveloperName(_configurationRepository.GetDeveloperName());
        }

        #endregion

        #region ChangeTheme Command

        private RelayCommand _changeThemeCommand;
        public RelayCommand ChangeThemeCommand
        {
            get
            {
                if (_changeThemeCommand == null)
                    _changeThemeCommand = new RelayCommand((param) => this.ChangeTheme(), (param) => this.CanChangeTheme());
                return _changeThemeCommand;
            }
        }

        public bool CanChangeTheme()
        {
            return true;
        }

        /// <summary>
        /// Executes the ChangeTheme command 
        /// </summary>
        public void ChangeTheme()
        {
            _navigator.ShowDialog<SelectThemeViewModel>();
        }

        #endregion

        #region ChangeScriptFolder Command

        private RelayCommand _changeScriptFolderCommand;

        public RelayCommand ChangeScriptFolderCommand
        {
            get
            {
                if (_changeScriptFolderCommand == null)
                    _changeScriptFolderCommand = new RelayCommand((param) => this.ChangeScriptFolder(), (param) => this.CanChangeScriptFolder());
                return _changeScriptFolderCommand;
            }
        }

        public bool CanChangeScriptFolder()
        {
            return true;
        }

        /// <summary>
        /// Executes the ChangeScriptFolder command 
        /// </summary>
        public void ChangeScriptFolder()
        {
            //throw new NotImplementedException();
        }

        #endregion

        private class ConfigurationInfo
        {
            public string ThemeName { get; set; }
            public string DeveloperName { get; set; }
            public Processor.Data.Models.ServerConnectionParameters ServerConnectionParams { get; set; }

        }
    }
}
