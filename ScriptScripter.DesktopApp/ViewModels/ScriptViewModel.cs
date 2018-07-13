using FluentValidation;
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
        public ScriptViewModel()
        {
            ViewTitle = "Create New Script";
        }

        public void Init(Processor.Data.Models.ScriptContainer scriptContainer)
        {
            _scriptContainer = scriptContainer;
        }
        public void Init(Processor.Data.Models.ScriptContainer scriptContainer, Processor.Data.Models.Script script)
        {
            this.Init(scriptContainer);

            ViewTitle = "Edit Script";
            _scriptInEdit = script;

            this.Comments = script.Notes;
            this.SQLStatement = script.SQLStatement;
        }

        [Ninject.Inject]
        public Processor.Data.Contracts.IScriptRepositoryFactory ScriptsRepoFactory { get; set; }

        [Ninject.Inject]
        public Processor.Data.Contracts.IConfigurationRepository ConfigurationRepository { get; set; }

        [Ninject.Inject]
        public Contracts.IViewModelFaultlessService ViewModelFaultlessService { get; set; }

        public string SQLStatement
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string Comments
        {
            get { return GetField<string>(); }
            set { SetField(value); }
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
        public void Commit()
        {
            if (this.GetValidationResult().IsValid)
            {
                this.ViewModelFaultlessService.TryExecuteSyncAsAsync(() => this.ExecuteCommit())
                    .OnSuccessAsync(() => Navigator.CloseDialog(this));
            }
            else
                ShowErrors = true;
        }
        private void ExecuteCommit()
        {
            var repo = this.ScriptsRepoFactory.GetScriptsRepository(_scriptContainer);

            if (_scriptInEdit == null)
            {
                repo.AddNewScript(new Processor.Data.Models.Script()
                {
                    SQLStatement = this.SQLStatement,
                    Notes = this.Comments,
                    DeveloperName = this.ConfigurationRepository.GetDeveloperName(),
                    ScriptDate = DateTime.Now
                });
            }
            else
            {
                _scriptInEdit.SQLStatement = this.SQLStatement;
                _scriptInEdit.Notes = this.Comments;

                //i debate on this... do we change the developer name?  i think so.
                _scriptInEdit.DeveloperName = this.ConfigurationRepository.GetDeveloperName();
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
            Navigator.CloseDialog(this);
        }

        #endregion

        #region Designer data
        protected override void OnLoadDesignData()
        {
            Comments = "new Table for storing mail accounts item #5495";
            SQLStatement = DesignTimeData.SQLStatements.Items[0];
        }

        #endregion

        #region Validation

        class NewScriptViewModelValidator : AbstractValidator<ScriptViewModel>
        {
            public NewScriptViewModelValidator()
            {

                RuleFor(obj => obj.SQLStatement).NotEmpty();
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
