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
    public class ApplyScriptsViewModel : ScriptScripterViewModelBase
    {
        private IEnumerable<Processor.Data.Models.Script> _scriptsToRun;
        private Processor.Data.Models.ScriptContainer _scriptContainer;
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private readonly Processor.Services.Contracts.IScriptingService _scriptingService;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;

#if DEBUG //exclude for release becasue for somereason IoC is using this ctor instead of the correct one
        public ApplyScriptsViewModel() : base(null) { }//Designer only   
#endif

        public ApplyScriptsViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Processor.Services.Contracts.IScriptingService scriptingService,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository,
            NLog.ILogger logger)
            : base(logger)
        {
            this._navigator = navigator;
            this._viewModelFaultlessService = viewModelFaultlessService;
            this._scriptingService = scriptingService;
            this._configurationRepository = configurationRepository;
        }

        public void Init(Processor.Data.Models.ScriptContainer scriptContainer)
        {
            _scriptContainer = scriptContainer;
            DatabaseName = scriptContainer.DatabaseName;
            ViewTitle = $"Apply Scripts to '{DatabaseName}'";
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
            var result = await _viewModelFaultlessService
                .TryExecuteSyncAsAsync(() => _scriptingService.GetScriptsThatNeedRun(this.GetDatabaseConnectionParameters(), _scriptContainer.ScriptFilePath));

            if (!result.WasSuccessful) return false;

            _scriptsToRun = result.ReturnValue;

            this.ProcessingSucceeded = false;
            this.ProcessingFailed = false;
            this.ProcessingMessage = null;

            LineItems = new System.Collections.ObjectModel.ObservableCollection<LineItem>();
            foreach (var script in _scriptsToRun)
            {
                LineItems.Add(new LineItem()
                {
                    ScriptId = script.ScriptId,
                    ScriptDate = script.ScriptDate.LocalDateTime.ToString(),
                    DeveloperName = script.DeveloperName,
                    SqlStatement = script.SqlStatement,
                    Notes = script.Notes
                });
            }
            return true;
        }

        public bool IsProcessingScripts
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string ProcessingMessage
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool ProcessingFailed
        {
            get { return GetField<bool>(); }
            set
            {
                if (SetField(value) && value)
                    ProcessingSucceeded = false;
            }
        }

        public bool ProcessingSucceeded
        {
            get { return GetField<bool>(); }
            set
            {
                if (SetField(value) && value)
                    ProcessingFailed = false;
            }
        }

        public string StatusMessage
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }


        #region EditScript Command

        private RelayCommand<LineItem> _editScriptCommand;
        public RelayCommand<LineItem> EditScriptCommand
        {
            get
            {
                if (_editScriptCommand == null)
                    _editScriptCommand = new RelayCommand<LineItem>((param) => this.EditScript(param), (param) => this.CanEditScript(param));
                return _editScriptCommand;
            }
        }

        public bool CanEditScript(LineItem lineItem)
        {
            return true;
        }

        /// <summary>
        /// Executes the EditScript command 
        /// </summary>
        public void EditScript(LineItem lineItem)
        {
            var script = _scriptsToRun.Single(s => s.ScriptId == lineItem.ScriptId);


            _navigator.ShowDialog<ScriptViewModel>(vm =>
            {
                vm.Init(_scriptContainer, script);
                vm.AllowApplyScripts = false;//we are already in apply, so if we allow this button they get into a popup nightmare
            });

            this.ReloadDataAsync();
        }

        #endregion

        #region ApplyScripts Command

        private RelayCommand _applyScriptsCommand;
        public RelayCommand ApplyScriptsCommand
        {
            get
            {
                if (_applyScriptsCommand == null)
                    _applyScriptsCommand = new RelayCommand((param) => this.ApplyScriptsAsync(), (param) => this.CanApplyScripts());
                return _applyScriptsCommand;
            }
        }

        public bool CanApplyScripts()
        {
            return !IsProcessingScripts && (LineItems?.Any() ?? false);
        }

        /// <summary>
        /// Executes the ApplyScripts command 
        /// </summary>
        public async void ApplyScriptsAsync()
        {
            var progress = new Progress<Processor.Dto.ApplyScriptProgress>();

            progress.ProgressChanged += (s, e) =>
            {
                var lineItem = LineItems.SingleOrDefault(r => r.ScriptId == e.Script.ScriptId);
                SelectedLineItem = lineItem;

                if (e.IsStarting)
                {
                    var total = e.ScriptsCompleted + e.ScriptsRemaining;
                    var current = e.ScriptsCompleted + 1;
                    ProcessingMessage = $"Processing Revision Dated {e.Script.ScriptDate.LocalDateTime.ToString("g")} ({current} of {total})";
                    lineItem.FailedProcessing = false;
                    lineItem.IsBeingProcessed = true;
                }
                else
                {
                    lineItem.IsBeingProcessed = false;
                    lineItem.HasBeenProcessed = true;
                }
            };

            IsProcessingScripts = true;
            try
            {
                var databaseParams = this.GetDatabaseConnectionParameters();

                var scripts = _scriptingService.GetScriptsThatNeedRun(databaseParams, _scriptContainer.ScriptFilePath);

                var result = await _scriptingService.ApplyScriptsToDatabaseAsync(databaseParams, scripts, progress);

                if (result.WasSuccessful)
                {
                    ProcessingSucceeded = true;
                    StatusMessage = "All scripts applied successfully!";
                    this.LineItems.Clear();
                    this.SelectedLineItem = null;
                }
                else
                {
                    SelectedLineItem.IsBeingProcessed = false;
                    SelectedLineItem.FailedProcessing = true;

                    ProcessingFailed = true;
                    StatusMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                _navigator.ShowDialog<ErrorViewModel>(vm => vm.LoadFromException(ex));
            }
            finally
            {
                IsProcessingScripts = false;
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
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
            return !IsProcessingScripts;
        }

        /// <summary>
        /// Executes the Cancel command 
        /// </summary>
        public void Cancel()
        {
            _navigator.CloseDialog(this);
        }

        #endregion

        #region RequestClose Command

        private RelayCommand<System.ComponentModel.CancelEventArgs> _requestCloseCommand;

        public RelayCommand<System.ComponentModel.CancelEventArgs> RequestCloseCommand
        {
            get
            {
                if (_requestCloseCommand == null)
                    _requestCloseCommand = new RelayCommand<System.ComponentModel.CancelEventArgs>((param) => this.RequestClose(param), (param) => this.CanRequestClose());
                return _requestCloseCommand;
            }
        }

        public bool CanRequestClose()
        {
            return true;
        }

        /// <summary>
        /// Executes the RequestClose command 
        /// </summary>
        public void RequestClose(System.ComponentModel.CancelEventArgs args)
        {
            if (this.IsProcessingScripts)
            {
                args.Cancel = true;

                _navigator.ShowDialog<MessageBoxViewModel>(vm =>
               {
                   vm.ViewTitle = "Cannot Close";
                   vm.Message = "Cannot close while processing";
                   vm.SetButtons(MessageBoxViewModel.MessageBoxButton.OK);
               });
            }
        }

        #endregion

        protected override void OnLoadDesignData()
        {
            LineItems = new System.Collections.ObjectModel.ObservableCollection<LineItem>();
            LineItems.Add(new LineItem()
            {
                ScriptId = Guid.NewGuid(),
                ScriptDate = DateTimeOffset.Now.ToString(),
                DeveloperName = "Cpt. Jack Sparrow",
                SqlStatement = DesignTimeData.SqlStatements.Items[0],
                Notes = @"Creating the new table"
            });
            LineItems.Add(new LineItem()
            {
                ScriptId = Guid.NewGuid(),
                ScriptDate = DateTimeOffset.Now.AddSeconds(-456879873).ToString(),
                DeveloperName = "Cpt. Jack Sparrow",
                SqlStatement = DesignTimeData.SqlStatements.Items[1],
                IsBeingProcessed = true,
                Notes = @"changing the SP cuz Jimmy told me to"
            });
            LineItems.Add(new LineItem()
            {
                ScriptId = Guid.NewGuid(),
                ScriptDate = DateTimeOffset.Now.AddSeconds(-9456879873).ToString(),
                DeveloperName = "Benny Jet",
                SqlStatement = DesignTimeData.SqlStatements.Items[2],
                HasBeenProcessed = true,
                Notes = @"Adding a new column because new columns are awesome, and i'm just going to keep typing this comment until i make it wrap.  note i say wrap, not rap, it can rap too, if you just give it the opportunity"
            });
        }

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

            public bool HasBeenProcessed
            {
                get { return GetField<bool>(); }
                set { SetField(value); }
            }

            public bool IsBeingProcessed
            {
                get { return GetField<bool>(); }
                set { SetField(value); }
            }

            public bool FailedProcessing
            {
                get { return GetField<bool>(); }
                set { SetField(value); }
            }
        }
        #endregion
    }
}
