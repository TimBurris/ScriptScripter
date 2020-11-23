using NinjaMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.ViewModels
{

    public class EditDatabaseViewModel : BaseDatabaseViewModel
    {
        private readonly Processor.Data.Contracts.IScriptContainerRepository _scriptContainerRepository;
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private Processor.Data.Models.ScriptContainer _scriptContainer;
        private Guid _containerUid;


        // public EditDatabaseViewModel() { }//Designer use   //removed because for somereason IoC is using this ctor instead of the correct one

        public EditDatabaseViewModel(Processor.Data.Contracts.IScriptContainerRepository scriptContainerRepository,
            NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            FileAndFolderDialog.Abstractions.IFileDialogService fileDialogService,
            DatabaseConnectionControlViewModel databaseConnectionControlVM,
            NLog.ILogger logger)
                : base(navigator, fileDialogService, databaseConnectionControlVM, logger)
        {
            ViewTitle = "Edit Database";
            this._scriptContainerRepository = scriptContainerRepository;
            this._navigator = navigator;
            this._viewModelFaultlessService = viewModelFaultlessService;
        }

        public void Init(Guid containerUid)
        {
            _containerUid = containerUid;
        }

        protected async override Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            var result = await _viewModelFaultlessService.TryExecuteSyncAsAsync(() => _scriptContainerRepository.GetByUid(_containerUid));
            if (!result.WasSuccessful)
                return false;

            if (result.ReturnValue == null)
            {
                this._navigator.ShowDialog<MessageBoxViewModel>(initAction: vm =>
                            vm.Init(title: "Not Found",
                            message: "Script Container not found",
                            buttons: MessageBoxViewModel.MessageBoxButton.OK)
                            );
                return false;
            }

            _scriptContainer = result.ReturnValue;

            this.DatabaseName = _scriptContainer.DatabaseName;
            this.ScriptFile = _scriptContainer.ScriptFilePath;

            var connectionParams = _scriptContainer.CustomServerConnectionParameters;

            if (connectionParams == null)
                this.UseDefaultDatabaseConnection = true;
            else
                this.DatabaseConnectionControlVM.Init(connectionParams);

            if (_scriptContainer.Tags != null)
                this.Tags = new System.Collections.ObjectModel.ObservableCollection<string>(_scriptContainer.Tags.ToList());

            return true;
        }

        public override void Save()
        {
            if (!GetValidationResult().IsValid)
            {
                ShowErrors = true;
                return;
            }

            var connectionParams = this.UseDefaultDatabaseConnection ? null : this.DatabaseConnectionControlVM.BuildConnectionParameters();

            _scriptContainer.DatabaseName = this.DatabaseName;
            _scriptContainer.ScriptFilePath = this.ScriptFile;
            _scriptContainer.CustomServerConnectionParameters = connectionParams;
            _scriptContainer.Tags = this.Tags.ToList();

            var result = _scriptContainerRepository.Update(_scriptContainer);

            if (result.WasSuccessful)
                _navigator.CloseDialog(this);
            else
                _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                    vm.Init(title: "Update Failed",
                       message: result.Message,
                       buttons: MessageBoxViewModel.MessageBoxButton.OK,
                       icon: MessageBoxViewModel.MessageBoxImage.Error
                       ));
        }
    }
}
