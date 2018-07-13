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
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private Processor.Data.Models.ScriptContainer _scriptContainer;
        private Guid _containerUid;


        public EditDatabaseViewModel() { }//Designer only

        public EditDatabaseViewModel(
            Contracts.IViewModelFaultlessService viewModelFaultlessService)
        {
            ViewTitle = "Edit Database";
            this._viewModelFaultlessService = viewModelFaultlessService;
        }

        public EditDatabaseViewModel(string defaultFileNamePattern,
             Contracts.IViewModelFaultlessService viewModelFaultlessService)
            : base(defaultFileNamePattern)
        {
            ViewTitle = "Edit Database";
            this._viewModelFaultlessService = viewModelFaultlessService;
        }

        public void Init(Guid containerUid)
        {
            _containerUid = containerUid;
        }

        protected async override Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            var result = await _viewModelFaultlessService.TryExecuteSyncAsAsync(() => this.ScriptContainerRepository.GetByUid(_containerUid));
            if (!result.WasSuccessful)
                return false;

            if (result.ReturnValue == null)
            {
                this.Navigator.ShowDialog<MessageBoxViewModel>(initAction: vm =>
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

            var result = this.ScriptContainerRepository.Update(_scriptContainer);

            if (result.WasSuccessful)
                Navigator.CloseDialog(this);
            else
                Navigator.ShowDialog<MessageBoxViewModel>(vm =>
                    vm.Init(title: "Update Failed",
                       message: result.Message,
                       buttons: MessageBoxViewModel.MessageBoxButton.OK,
                       icon: MessageBoxViewModel.MessageBoxImage.Error
                       ));
        }
    }
}
