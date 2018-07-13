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

        public DatabaseScriptsViewModel() { }

        public void Init(Processor.Data.Models.ScriptContainer scriptContainer)
        {
            _scriptContainer = scriptContainer;
            DatabaseName = _scriptContainer.DatabaseName;
        }

        [Ninject.Inject]
        public Processor.Data.Contracts.IConfigurationRepository ConfigurationRepository { get; set; }

        [Ninject.Inject]
        public Processor.Data.Contracts.IRevisionRepository RevisionRepository { get; set; }

        [Ninject.Inject]
        public Processor.Data.Contracts.IScriptRepositoryFactory ScriptRepoFactory { get; set; }

        [Ninject.Inject]
        public Processor.Services.Contracts.IScriptingService ScriptingService { get; set; }

        [Ninject.Inject]
        public Contracts.IViewModelFaultlessService ViewModelFaultlessService { get; set; }

        private Processor.Services.Contracts.IEventNotificationService _eventNotificationService;

        [Ninject.Inject]
        public Processor.Services.Contracts.IEventNotificationService EventNotificationService
        {
            get { return _eventNotificationService; }
            set
            {
                if (_eventNotificationService != null)
                {
                    _eventNotificationService.ServerConnectionChanged -= _eventNotificationService_ServerConnectionChanged;
                    _eventNotificationService.ScriptContainerContentsChanged -= _eventNotificationService_ScriptContainerContentsChanged;
                }

                _eventNotificationService = value;

                if (_eventNotificationService != null)
                {
                    _eventNotificationService.ServerConnectionChanged += _eventNotificationService_ServerConnectionChanged;
                    _eventNotificationService.ScriptContainerContentsChanged += _eventNotificationService_ScriptContainerContentsChanged;
                }
            }
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
                ?? this.ConfigurationRepository.GetServerConnectionParameters();

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

            var scriptRepo = this.ScriptRepoFactory.GetScriptsRepository(_scriptContainer);
            var databaseConnectionParams = this.GetDatabaseConnectionParameters();
            var t = this.ViewModelFaultlessService.TryExecuteSyncAsAsync(() => scriptRepo.GetLastScript());
            var t3 = this.ViewModelFaultlessService.TryExecuteAsync(() => ScriptingService.TestDatabaseConnectionAsync(databaseConnectionParams));

            await Task.WhenAll(t, t3);

            if (!t3.Result.WasSuccessful)
                return false;

            lastScript = t.Result.ReturnValue;
            connectionResult = t3.Result.ReturnValue;

            if (connectionResult.WasSuccessful)
                lastRevision = (await this.ViewModelFaultlessService.TryExecuteSyncAsAsync(() => RevisionRepository.GetLastRevision(databaseConnectionParams)))
                    .ReturnValue;
            else
                lastRevision = null;


            //TODO: these properties are poorly named, inconsistent with models. fix them.  their vm names look good... the model names suck
            if (lastRevision != null)
            {
                DatabaseRevisionNumber = lastRevision.RevisionNumber.ToString();
                DatabaseLastRevisionDate = lastRevision.RunDate.ToString();
                DatabaseLastRevisionByDeveloper = lastRevision.RunByDeveloperName;
            }
            if (lastScript != null)
            {
                ScriptFileRevisionNumber = lastScript.RevisionNumber.ToString();
                ScriptFileLastRevisionDate = lastScript.ScriptDate.ToString();
                ScriptFileLastRevisionByDeveloper = lastScript.DeveloperName;
            }

            IsUpToDate = false;
            IsNotConnected = false;
            IsOutOfDate = false;
            IsDatabaseNewer = false;

            if (!connectionResult.WasSuccessful)
                IsNotConnected = true;
            else if (lastRevision == null && lastScript == null)
                IsUpToDate = true;
            else if (lastRevision == null)
                IsOutOfDate = true;
            else if (lastScript == null)
                IsDatabaseNewer = true;
            else if (lastScript.RevisionNumber > lastRevision.RevisionNumber)
                IsOutOfDate = true;
            else if (lastScript.RevisionNumber == lastRevision.RevisionNumber)
                IsUpToDate = true;
            else
                IsDatabaseNewer = true;

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

        public string ScriptFileRevisionNumber
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

        public string DatabaseRevisionNumber
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
            Navigator.ShowDialog<ScriptViewModel>(vm => vm.Init(_scriptContainer));
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
            Navigator.ShowDialog<ApplyScriptsViewModel>(vm => vm.Init(_scriptContainer));

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
            ScriptFileRevisionNumber = "378";
            ScriptFileLastRevisionDate = DateTime.Now.AddDays(-37).ToString();
            ScriptFileLastRevisionByDeveloper = "Cpt. Jack Sparrow";

            DatabaseRevisionNumber = "217";
            DatabaseLastRevisionDate = DateTime.Now.AddDays(-3).ToString();
            DatabaseLastRevisionByDeveloper = "Jimmy James";

        }
        #endregion

    }
}
