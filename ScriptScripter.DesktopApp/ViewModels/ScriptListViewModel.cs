using NinjaMvvm;
using NinjaMvvm.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class ScriptListViewModel : ScriptScripterViewModelBase
    {
        private Processor.Data.Models.ScriptContainer _scriptContainer;
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private readonly Processor.Data.Contracts.IScriptRepositoryFactory _scriptRepoFactory;
        private readonly Processor.Services.Contracts.IScriptingService _scriptingService;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;


        public ScriptListViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Processor.Data.Contracts.IScriptRepositoryFactory scriptRepoFactory,
            Processor.Services.Contracts.IScriptingService scriptingService,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository,
            NLog.ILogger logger)
            : base(logger)
        {
            this._navigator = navigator;
            this._viewModelFaultlessService = viewModelFaultlessService;
            _scriptRepoFactory = scriptRepoFactory;
            this._scriptingService = scriptingService;
            this._configurationRepository = configurationRepository;
        }

        public void Init(Processor.Data.Models.ScriptContainer scriptContainer)
        {
            _scriptContainer = scriptContainer;
            DatabaseName = scriptContainer.DatabaseName;
            ViewTitle = $"Scripts For '{DatabaseName}'";
        }

        public string DatabaseName
        {
            get { return GetField<string>(); }
            set { SetField(value); }
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

        public System.Collections.ObjectModel.ObservableCollection<LineItem> LineItems
        {
            get { return GetField<System.Collections.ObjectModel.ObservableCollection<LineItem>>(); }
            set { SetField(value); }
        }

        public LineItem SelectedLineItem
        {
            get { return GetField<LineItem>(); }
            set { SetField(value); }
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            var scriptRepo = _scriptRepoFactory.GetScriptsRepository(_scriptContainer.ScriptFilePath);
            var scriptsResult = await _viewModelFaultlessService.TryExecuteSyncAsAsync(() => scriptRepo.GetAllScripts());

            if (!scriptsResult.WasSuccessful)
                return false;
            var allScripts = scriptsResult.ReturnValue;

            var toRunResult = await _viewModelFaultlessService
                                    .TryExecuteSyncAsAsync(() => _scriptingService.GetScriptsThatNeedRun(this.GetDatabaseConnectionParameters(), _scriptContainer.ScriptFilePath));

            HashSet<Guid> scriptsToRun;
            //it's ok if we don't have this, we can continue without it
            if (toRunResult.WasSuccessful)
            {
                scriptsToRun = toRunResult.ReturnValue.Select(x => x.ScriptId).ToHashSet();
            }
            else
            {
                scriptsToRun = new HashSet<Guid>();
            }

            LineItems = new System.Collections.ObjectModel.ObservableCollection<LineItem>();
            foreach (var script in allScripts.OrderBy(x => x.ScriptDate))
            {
                LineItems.Add(new LineItem()
                {
                    ScriptId = script.ScriptId,
                    ScriptDate = script.ScriptDate.LocalDateTime.ToString(),
                    DeveloperName = script.DeveloperName,
                    SqlStatement = script.SqlStatement,
                    Notes = script.Notes,
                    HasBeenApplied = scriptsResult.WasSuccessful && !scriptsToRun.Contains(script.ScriptId)
                });
            }
            return true;
        }
        #region Open Command

        private RelayCommand<LineItem> _openCommand;
        public RelayCommand<LineItem> OpenCommand
        {
            get
            {
                if (_openCommand == null)
                    _openCommand = new RelayCommand<LineItem>((param) => this.Open(param), (param) => this.CanOpen());
                return _openCommand;
            }
        }

        public bool CanOpen()
        {
            return true;
        }

        /// <summary>
        /// Executes the Open command 
        /// </summary>
        public void Open(LineItem lineItem)
        {
            _navigator.NavigateTo<DatabaseScriptsViewModel>(vm => vm.Init(scriptContainer: _scriptContainer));
        }

        #endregion

        #region LineItem Class
        public class LineItem : NotificationBase
        {
            public Guid ScriptId
            {
                get { return GetField<Guid>(); }
                set { SetField(value); }
            }

            public string ScriptDate
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public string DeveloperName
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public string SqlStatement
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public string Notes
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public bool HasBeenApplied
            {
                get { return GetField<bool>(); }
                set { SetField(value); }
            }
        }
        #endregion
    }
}
