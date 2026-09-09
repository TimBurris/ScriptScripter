using NinjaMvvm.Wpf;
using ScriptScripter.Processor.Data.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class MainViewModel : ScriptScripterViewModelBase
    {
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Processor.Services.Contracts.IScriptContainerWatcherService _scriptContainerWatcherService;
        private readonly IScriptContainerRepository _scriptsContainerRepository;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private readonly Processor.Services.Contracts.IConfigurationFileUpgradeService _configurationFileUpgradeService;
        private readonly Contracts.IThemeService _themeService;
        private readonly Processor.Services.Contracts.IEventNotificationService _eventNotificationService;
        private readonly Processor.Services.Contracts.IWorktreeResolverService _worktreeResolverService;

        //public MainViewModel() { }//designer only   //removed because for somereason IoC is using this ctor instead of the correct one

        public MainViewModel(
            NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Processor.Services.Contracts.IScriptContainerWatcherService scriptContainerWatcherService,
            Processor.Data.Contracts.IScriptContainerRepository scriptsContainerRepository,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Processor.Services.Contracts.IEventNotificationService eventNotificationService,
            Processor.Services.Contracts.IConfigurationFileUpgradeService configurationFileUpgradeService,
            Contracts.IThemeService themeService,
            Processor.Services.Contracts.IWorktreeResolverService worktreeResolverService,
            NLog.ILogger logger)
            : base(logger)
        {
            ViewTitle = "ScriptScripter";
            this._navigator = navigator;
            this._scriptContainerWatcherService = scriptContainerWatcherService;
            _scriptsContainerRepository = scriptsContainerRepository;
            this._configurationRepository = configurationRepository;
            this._viewModelFaultlessService = viewModelFaultlessService;
            this._eventNotificationService = eventNotificationService;
            this._configurationFileUpgradeService = configurationFileUpgradeService;
            this._themeService = themeService;
            _worktreeResolverService = worktreeResolverService;
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


        //HACK: this is a hack to get the Add New Script dialog to show when the app is started with the -a param
        internal void AddNewScriptForContainer(string addScriptContainerPath, bool useClipboardForNewScript)
        {
            var allContainers = _scriptsContainerRepository.GetAll().ToList();

            var exactMatches = allContainers
                .Where(x => PathsMatch(x.ScriptContainerPath, addScriptContainerPath))
                .ToList();

            Processor.Data.Models.ScriptContainer scriptContainer;

            if (exactMatches.Count == 1)
            {
                scriptContainer = exactMatches[0];
            }
            else if (exactMatches.Count > 1)
            {
                scriptContainer = this.PickScriptContainer(exactMatches);
                if (scriptContainer == null)
                    return; //user cancelled the picker
            }
            else
            {
                scriptContainer = this.ResolveWorktreeScriptContainer(addScriptContainerPath, allContainers);
                if (scriptContainer == null)
                    return; //either not found (dialog already shown) or the picker was cancelled
            }

            string sqlScript = null;
            if (useClipboardForNewScript)
            {
                sqlScript = _viewModelFaultlessService.TryExecute(() => System.Windows.Clipboard.GetText())?.ReturnValue;
            }

            Task.Run(async () =>
            {
                //the delay is really not necessary, but it looks a little better if we wait a second before showing the dialog
                await Task.Delay(1000);
                //use dispatcher because we just Ran a task which could mean we are on a different thread
                App.Current.Dispatcher.Invoke(() => _viewModelFaultlessService.TryExecute(() => _navigator.ShowDialog<ScriptViewModel>(vm => vm.Init(scriptContainer, sqlScript))));
            });
        }

        /// <summary>
        /// Called when <paramref name="addScriptContainerPath"/> matched none of the configured containers exactly.
        /// Checks whether it sits inside a git worktree and, if so, re-roots it under the main checkout and repeats
        /// the exact-match lookup against the candidate path. Returns a transient, in-memory clone of the matched
        /// container (same DatabaseName / connection params, ScriptContainerPath pointed at the worktree path) -
        /// nothing is written to the configuration file. Returns null (having shown a dialog, or after the user
        /// cancelled the picker) when no container can be resolved.
        /// </summary>
        private Processor.Data.Models.ScriptContainer ResolveWorktreeScriptContainer(string addScriptContainerPath, System.Collections.Generic.List<Processor.Data.Models.ScriptContainer> allContainers)
        {
            var worktreeResolution = _worktreeResolverService.ResolveWorktreeCandidatePath(addScriptContainerPath);

            if (!worktreeResolution.IsWorktree)
            {
                this.ShowContainerNotFoundDialog(addScriptContainerPath, allContainers);
                return null;
            }

            var candidateMatches = allContainers
                .Where(x => PathsMatch(x.ScriptContainerPath, worktreeResolution.CandidatePath))
                .ToList();

            Processor.Data.Models.ScriptContainer matchedContainer;

            if (candidateMatches.Count == 1)
            {
                matchedContainer = candidateMatches[0];
            }
            else if (candidateMatches.Count > 1)
            {
                matchedContainer = this.PickScriptContainer(candidateMatches);
                if (matchedContainer == null)
                    return null; //user cancelled the picker
            }
            else
            {
                this.ShowContainerNotFoundDialog(addScriptContainerPath, allContainers, worktreeResolution);
                return null;
            }

            //transient, in-memory clone: same DatabaseName / connection params, but pointed at the original
            //worktree path. Nothing is written to the configuration file - no new list entry, no watcher,
            //no ContainerUid persisted.
            return new Processor.Data.Models.ScriptContainer()
            {
                DatabaseName = matchedContainer.DatabaseName,
                CustomServerConnectionParameters = matchedContainer.CustomServerConnectionParameters,
                ScriptContainerPath = addScriptContainerPath,
            };
        }

        private Processor.Data.Models.ScriptContainer PickScriptContainer(System.Collections.Generic.List<Processor.Data.Models.ScriptContainer> options)
        {
            var pickerViewModel = _navigator.ShowDialog<SelectScriptContainerViewModel>(vm => vm.Init(options));
            return pickerViewModel.SelectedContainer;
        }

        private void ShowContainerNotFoundDialog(string addScriptContainerPath, System.Collections.Generic.List<Processor.Data.Models.ScriptContainer> allContainers, Processor.Dto.WorktreeResolutionResult worktreeResolution = null)
        {
            var allContainersMessage = string.Join("\r\n", allContainers.Select(x => x.ScriptContainerPath));

            var detailsMessage = worktreeResolution == null
                ? $"The path specified was: '{addScriptContainerPath}' which does not match any of these:\r\n--------------------\r\n{allContainersMessage}"
                : $"The path specified was: '{addScriptContainerPath}', detected as a git worktree.\r\n"
                    + $"Worktree root: '{worktreeResolution.WorktreeRoot}'\r\n"
                    + $"Resolved main checkout root: '{worktreeResolution.MainCheckoutRoot}'\r\n"
                    + $"Candidate path searched for: '{worktreeResolution.CandidatePath}', which does not match any of these:\r\n--------------------\r\n{allContainersMessage}";

            _navigator.ShowDialog<MessageBoxViewModel>(initAction: vm =>
            {
                vm.Init("Error", "The specified script container was not found", MessageBoxViewModel.MessageBoxButton.OK, MessageBoxViewModel.MessageBoxImage.Exclamation);
                vm.MoreDetailsMessage = detailsMessage;
                vm.CanShowMoreDetails = true;
                vm.MoreDetailsCaption = "More details";
            });
        }

        /// <summary>
        /// Case-insensitive path comparison with trailing directory separators trimmed.
        /// </summary>
        private static bool PathsMatch(string pathA, string pathB)
        {
            return string.Equals(TrimTrailingSeparators(pathA), TrimTrailingSeparators(pathB), StringComparison.OrdinalIgnoreCase);
        }

        private static string TrimTrailingSeparators(string path)
        {
            return path?.TrimEnd('\\', '/');
        }

        private class ConfigurationInfo
        {
            public string ThemeName { get; set; }
            public string DeveloperName { get; set; }
            public Processor.Data.Models.ServerConnectionParameters ServerConnectionParams { get; set; }

        }
    }
}
