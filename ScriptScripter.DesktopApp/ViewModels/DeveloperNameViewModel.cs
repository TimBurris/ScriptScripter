using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaultlessExecution.Extensions;
using NinjaMvvm;
using NinjaMvvm.Wpf;
using System.Threading;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class DeveloperNameViewModel : ScriptScripterViewModelBase
    {
        [Ninject.Inject]
        public Processor.Data.Contracts.IConfigurationRepository ConfigurationRepository { get; set; }

        [Ninject.Inject]
        public Contracts.IViewModelFaultlessService ViewModelFaultlessService { get; set; }

        public DeveloperNameViewModel()
        {
            ViewTitle = "Your Name";
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            var result = await this.ViewModelFaultlessService
                                   .TryExecuteSyncAsAsync(() => this.ConfigurationRepository.GetDeveloperName());

            if (!result.WasSuccessful)
                return false;

            DeveloperName = result.ReturnValue;
            return true;
        }

        #region Binding props and Lists

        public string DeveloperName
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        #endregion

        #region Save Command

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new RelayCommand((param) => this.Save(), (param) => this.CanSave());
                return _saveCommand;
            }
        }

        public bool CanSave()
        {
            return true;
        }

        /// <summary>
        /// Executes the Save command 
        /// </summary>
        public void Save()
        {

            if (!GetValidationResult().IsValid)
            {
                ShowErrors = true;
                return;
            }

            this.ViewModelFaultlessService
                .TryExecute(() => this.ConfigurationRepository.SetDeveloperName(DeveloperName))
                .OnSuccess(() => Navigator.CloseDialog(this));
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

        #region Validation

        class DeveloperNameViewModelValidator : AbstractValidator<DeveloperNameViewModel>
        {
            public DeveloperNameViewModelValidator()
            {
                RuleFor(obj => obj.DeveloperName).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new DeveloperNameViewModelValidator();
        }

        #endregion

        #region Designer data
        protected override void OnLoadDesignData()
        {
            DeveloperName = "DumpsterNinja";
        }

        #endregion
    }
}
