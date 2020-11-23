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
    public class DatabaseScriptsViewModel : ScriptScripterViewModelBase
    {
        private Processor.Data.Models.ScriptContainer _scriptContainer;
        private readonly Processor.Services.Contracts.IEventNotificationService _eventNotificationService;
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;
        private readonly Processor.Data.Contracts.IRevisionRepository _revisionRepository;
        private readonly Processor.Data.Contracts.IScriptRepositoryFactory _scriptRepoFactory;
        private readonly Processor.Services.Contracts.IScriptingService _scriptingService;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;

        //public DatabaseScriptsViewModel() { }//designer only   //removed because for somereason IoC is using this ctor instead of the correct one
        public DatabaseScriptsViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository,
            Processor.Data.Contracts.IRevisionRepository revisionRepository,
            Processor.Data.Contracts.IScriptRepositoryFactory scriptRepoFactory,
            Processor.Services.Contracts.IScriptingService scriptingService,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Processor.Services.Contracts.IEventNotificationService eventNotificationService,
            NLog.ILogger logger)
            : base(logger)
        {
            this._navigator = navigator;
            this._configurationRepository = configurationRepository;
            this._revisionRepository = revisionRepository;
            this._scriptRepoFactory = scriptRepoFactory;
            this._scriptingService = scriptingService;
            this._viewModelFaultlessService = viewModelFaultlessService;
            this._eventNotificationService = eventNotificationService;

            if (_eventNotificationService != null)
            {
                _eventNotificationService.ServerConnectionChanged += _eventNotificationService_ServerConnectionChanged;
                _eventNotificationService.ScriptContainerContentsChanged += _eventNotificationService_ScriptContainerContentsChanged;
            }
        }

        public void Init(Processor.Data.Models.ScriptContainer scriptContainer)
        {
            _scriptContainer = scriptContainer;
            DatabaseName = _scriptContainer.DatabaseName;
        }

        private void _eventNotificationService_ScriptContainerContentsChanged(object sender, Processor.EventArgs<Processor.Data.Models.ScriptContainer> e)
        {
            if (e.EventData.DatabaseName == this.DatabaseName)
                this.ReloadDataAsync();
        }

        private void _eventNotificationService_ServerConnectionChanged(object sender, EventArgs e)
        {
            this.ReloadDataAsync();
        }

        private Processor.Data.Models.DatabaseConnectionParameters GetDatabaseConnectionParameters()
        {
            var connParams = _scriptContainer.CustomServerConnectionParameters
                ?? _configurationRepository.GetServerConnectionParameters();

            return new Processor.Data.Models.DatabaseConnectionParameters(copyFrom: connParams)
            {
                DatabaseName = this.DatabaseName
            };
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            Processor.Data.Models.Script lastScript = null;
            Processor.Data.Models.Revision lastRevision = null;
            Processor.Dto.ActionResult connectionResult;

            //if they have customconnection show it, else show null
            this.ServerConnectionInfo = _scriptContainer.CustomServerConnectionParameters?.ToString();

            var scriptRepo = _scriptRepoFactory.GetScriptsRepository(_scriptContainer.ScriptFilePath);
            var databaseConnectionParams = this.GetDatabaseConnectionParameters();
            var t = _viewModelFaultlessService.TryExecuteSyncAsAsync(() => scriptRepo.GetLastScript());
            var t3 = _viewModelFaultlessService.TryExecuteAsync(() => _scriptingService.TestDatabaseConnectionAsync(databaseConnectionParams));

            await Task.WhenAll(t, t3);

            if (!t3.Result.WasSuccessful)
                return false;

            lastScript = t.Result.ReturnValue;
            connectionResult = t3.Result.ReturnValue;

            if (connectionResult.WasSuccessful)
                lastRevision = (await _viewModelFaultlessService.TryExecuteSyncAsAsync(() => _revisionRepository.GetLastRevision(databaseConnectionParams)))
                    .ReturnValue;
            else
                lastRevision = null;


            //TODO: these properties are poorly named, inconsistent with models. fix them.  their vm names look good... the model names suck
            if (lastRevision != null)
            {
                DatabaseLastRevisionDate = lastRevision.ScriptDate.LocalDateTime.ToString();
                DatabaseLastRevisionByDeveloper = lastRevision.RunByDeveloperName;
            }
            if (lastScript != null)
            {
                ScriptFileLastRevisionDate = lastScript.ScriptDate.LocalDateTime.ToString();
                ScriptFileLastRevisionByDeveloper = lastScript.DeveloperName;
            }

            IsUpToDate = false;
            IsNotConnected = false;
            IsOutOfDate = false;
            IsDatabaseNewer = false;

            if (!connectionResult.WasSuccessful)
            {
                IsNotConnected = true;
            }
            else
            {
                var state = _scriptingService.GetDatabaseScriptState(databaseConnectionParams, _scriptContainer.ScriptFilePath);
                switch (state)
                {
                    case Processor.Services.Contracts.DatabaseScriptStates.Newer:
                        IsDatabaseNewer = true;
                        break;
                    case Processor.Services.Contracts.DatabaseScriptStates.OutOfdate:
                        IsOutOfDate = true;
                        break;
                    case Processor.Services.Contracts.DatabaseScriptStates.UpToDate:
                        IsUpToDate = true;
                        break;

                    default:
                        throw new NotSupportedException($"{state} is not supported by this view");
                }
            }

            return true;
        }

        #region Binding props and Lists

        public bool IsUpToDate
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public bool IsOutOfDate
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public bool IsNotConnected
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public bool IsDatabaseNewer
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string DatabaseName
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string ScriptFileLastRevisionDate
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string ScriptFileLastRevisionByDeveloper
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string DatabaseLastRevisionDate
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string DatabaseLastRevisionByDeveloper
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string ServerConnectionInfo
        {
            get { return GetField<string>(); }
            set
            {
                if (SetField(value))
                    OnPropertyChanged(nameof(HasServerConnectionInfo));
            }
        }

        public bool HasServerConnectionInfo { get { return !string.IsNullOrEmpty(ServerConnectionInfo); } }
        #endregion

        #region AddNewScript Command

        private RelayCommand _addNewScriptCommand;
        public RelayCommand AddNewScriptCommand
        {
            get
            {
                if (_addNewScriptCommand == null)
                    _addNewScriptCommand = new RelayCommand((param) => this.AddNewScript(), (param) => this.CanAddNewScript());
                return _addNewScriptCommand;
            }
        }

        public bool CanAddNewScript()
        {
            return true;
        }

        /// <summary>
        /// Executes the AddNewScript command 
        /// </summary>
        public void AddNewScript()
        {
            _navigator.ShowDialog<ScriptViewModel>(vm => vm.Init(_scriptContainer));
        }

        #endregion

        #region ApplyScripts Command

        private RelayCommand _applyScriptsCommand;

        public RelayCommand ApplyScriptsCommand
        {
            get
            {
                if (_applyScriptsCommand == null)
                    _applyScriptsCommand = new RelayCommand((param) => this.ApplyScripts(), (param) => this.CanApplyScripts());
                return _applyScriptsCommand;
            }
        }

        public bool CanApplyScripts()
        {
            return true;
        }

        /// <summary>
        /// Executes the ApplyRevisions command 
        /// </summary>
        public void ApplyScripts()
        {
            _navigator.ShowDialog<ApplyScriptsViewModel>(vm => vm.Init(_scriptContainer));

            this.ReloadDataAsync();
        }

        #endregion

        #region Designtime Data
        protected override void OnLoadDesignData()
        {
            var x = new Random();
            var v = x.Next(minValue: 1, maxValue: 4);
            switch (v)
            {
                case 1:
                    IsDatabaseNewer = true;
                    break;
                case 2:
                    IsNotConnected = true;
                    break;
                case 3:
                    IsOutOfDate = true;
                    break;
                case 4:
                    IsUpToDate = true;
                    break;
                default:
                    //?
                    break;
            }

            this.ServerConnectionInfo = "deserver\\SQL (sa ********)";
            DatabaseName = "SampleDatabase";
            ScriptFileLastRevisionDate = DateTimeOffset.Now.AddDays(-37).ToString();
            ScriptFileLastRevisionByDeveloper = "Cpt. Jack Sparrow";

            DatabaseLastRevisionDate = DateTimeOffset.Now.AddDays(-3).ToString();
            DatabaseLastRevisionByDeveloper = "Jimmy James";

        }
        #endregion

    }
}
