﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaMvvm;
using NinjaMvvm.Wpf;
using FaultlessExecution.Extensions;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class ScriptViewModel : ScriptScripterViewModelBase
    {
        private Processor.Data.Models.ScriptContainer _scriptContainer;
        private Processor.Data.Models.Script _scriptInEdit;
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;
        private readonly Processor.Data.Contracts.IScriptRepositoryFactory _scriptsRepoFactory;
        private readonly Processor.Services.Contracts.IScriptWarningService _scriptWarningService;
        private readonly Processor.Services.Contracts.IScriptingService _scriptingService;


#if DEBUG //exclude for release becasue for somereason IoC is using this ctor instead of the correct one
        public ScriptViewModel() : base(null) { }//Designer only   
#endif

        public ScriptViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository,
            Processor.Data.Contracts.IScriptRepositoryFactory scriptsRepoFactory,
            Processor.Services.Contracts.IScriptWarningService scriptWarningService,
            Processor.Services.Contracts.IScriptingService scriptingService,
            NLog.ILogger logger)
            : base(logger)
        {
            this._navigator = navigator;
            this._viewModelFaultlessService = viewModelFaultlessService;
            this._configurationRepository = configurationRepository;
            this._scriptsRepoFactory = scriptsRepoFactory;
            _scriptWarningService = scriptWarningService;
            _scriptingService = scriptingService;
            this.PropertyChanged += ScriptViewModel_PropertyChanged;

            this.AllowApplyScripts = true;//default is on, but some peeps might want to disable that (like if you are editing a script from the apply scripts view)
        }

        private void ScriptViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SqlStatement))
            {
                this.CheckForWarnings();
            }
        }

        public void Init(Processor.Data.Models.ScriptContainer scriptContainer)
        {
            _scriptContainer = scriptContainer;
            ViewTitle = $"Create New Script - {scriptContainer.DatabaseName}";
            this.DatabaseName = scriptContainer.DatabaseName;
        }

        public void Init(Processor.Data.Models.ScriptContainer scriptContainer, Processor.Data.Models.Script script)
        {
            this.Init(scriptContainer);

            ViewTitle = $"Edit Script - {scriptContainer.DatabaseName}";
            _scriptInEdit = script;

            this.Comments = script.Notes;
            this.SqlStatement = script.SqlStatement;
            this.DatabaseName = scriptContainer.DatabaseName;
        }

        protected override void OnUnloaded()
        {
            this.PropertyChanged -= ScriptViewModel_PropertyChanged;
            base.OnUnloaded();
        }


        public bool AllowApplyScripts
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string DatabaseName
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }
        public string SqlStatement
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool HasSqlWarnings
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string SqlWarnings
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string Comments
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }
        private void CheckForWarnings()
        {
            var scriptWarnings = _scriptWarningService.CheckSql(this.SqlStatement);
            if (scriptWarnings.Any())
            {
                this.HasSqlWarnings = true;
                this.SqlWarnings = string.Join("\r\n", scriptWarnings);
            }
            else
            {
                this.HasSqlWarnings = false;
                this.SqlWarnings = null;
            }

        }
        #region Commit Command

        private RelayCommand _commitCommand;
        public RelayCommand CommitCommand
        {
            get
            {
                if (_commitCommand == null)
                    _commitCommand = new RelayCommand((param) => this.Commit(), (param) => this.CanCommit());
                return _commitCommand;
            }
        }

        public bool CanCommit()
        {
            return true;
        }

        /// <summary>
        /// Executes the Commit command 
        /// </summary>
        public bool Commit()
        {
            if (this.GetValidationResult().IsValid)
            {
                if (!_scriptingService.TestScriptContainerExists(_scriptContainer.ScriptContainerPath).WasSuccessful)
                {
                    var result = _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                    {
                        vm.ViewTitle = "Script Container Missing";
                        vm.Message = $"The Script Container '{_scriptContainer.ScriptContainerPath}' does not exist, would you like to create it?";
                        vm.SetButtons(MessageBoxViewModel.MessageBoxButton.YesNo);
                    });

                    if (result.ViewResult == MessageBoxViewModel.MessageBoxResult.Yes)
                    {
                        var createResult = _scriptingService.TryCreateScriptContainer(_scriptContainer.ScriptContainerPath);

                        if (!createResult.WasSuccessful)
                        {
                            _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                            {
                                vm.ViewTitle = "Failed";
                                vm.Message = "Could not create the script container";
                                vm.SetButtons(MessageBoxViewModel.MessageBoxButton.OK);
                            });

                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                _viewModelFaultlessService.TryExecuteSyncAsAsync(() => this.ExecuteCommit())
                    .OnSuccessAsync(() => _navigator.CloseDialog(this));

                return true;
            }
            else
            {
                ShowErrors = true;
                return false;
            }
        }
        private void ExecuteCommit()
        {

            var repo = _scriptsRepoFactory.GetScriptsRepository(_scriptContainer.ScriptContainerPath);

            if (_scriptInEdit == null)
            {
                repo.AddNewScript(new Processor.Data.Models.Script()
                {
                    SqlStatement = this.SqlStatement,
                    Notes = this.Comments,
                    DeveloperName = _configurationRepository.GetDeveloperName(),
                    ScriptDate = DateTimeOffset.Now
                });
            }
            else
            {
                _scriptInEdit.SqlStatement = this.SqlStatement;
                _scriptInEdit.Notes = this.Comments;

                //i debate on this... do we change the developer name?  i think so.
                _scriptInEdit.DeveloperName = _configurationRepository.GetDeveloperName();
                //do not chang ethe date

                repo.UpdateScript(_scriptInEdit);
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


        #region CommitAndApply Command

        private RelayCommand _commitAndApplyCommand;
        public RelayCommand CommitAndApplyCommand
        {
            get
            {
                if (_commitAndApplyCommand == null)
                    _commitAndApplyCommand = new RelayCommand((param) => this.CommitAndApply(), (param) => this.CanCommitAndApply());
                return _commitAndApplyCommand;
            }
        }

        public bool CanCommitAndApply()
        {
            return this.AllowApplyScripts;
        }

        /// <summary>
        /// Executes the CommitAndApply command 
        /// </summary>
        public void CommitAndApply()
        {
            if (this.Commit())
            {
                _navigator.ShowDialog<ApplyScriptsViewModel>(vm => vm.Init(_scriptContainer));

                _navigator.CloseDialog(this);
            }
        }

        #endregion

        #region Designer data
        protected override void OnLoadDesignData()
        {
            Comments = "new Table for storing mail accounts item #5495";
            SqlStatement = DesignTimeData.SqlStatements.Items[0];

            //SqlWarnings = "";
            //HasSqlWarnings = true;
        }

        #endregion

        #region Validation

        class NewScriptViewModelValidator : AbstractValidator<ScriptViewModel>
        {
            public NewScriptViewModelValidator()
            {

                RuleFor(obj => obj.SqlStatement).NotEmpty();
                RuleFor(obj => obj.Comments).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new NewScriptViewModelValidator();
        }
        #endregion


    }
}
