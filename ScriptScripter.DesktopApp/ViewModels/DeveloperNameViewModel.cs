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
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private readonly Processor.Services.Contracts.IScriptingService _scriptingService;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;

        public DeveloperNameViewModel() { }//Designer use

        public DeveloperNameViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Processor.Services.Contracts.IScriptingService scriptingService,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository)
        {
            ViewTitle = "Your Name";
            this._navigator = navigator;
            this._viewModelFaultlessService = viewModelFaultlessService;
            this._scriptingService = scriptingService;
            this._configurationRepository = configurationRepository;
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            var result = await _viewModelFaultlessService
                                   .TryExecuteSyncAsAsync(() => _configurationRepository.GetDeveloperName());

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

            _viewModelFaultlessService
                .TryExecute(() => _configurationRepository.SetDeveloperName(DeveloperName))
                .OnSuccess(() => _navigator.CloseDialog(this));
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
