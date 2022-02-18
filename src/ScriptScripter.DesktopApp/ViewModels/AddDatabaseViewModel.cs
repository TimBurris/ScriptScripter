using NinjaMvvm;
using NinjaMvvm.Wpf;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class AddDatabaseViewModel : BaseDatabaseViewModel
    {
        private readonly Processor.Data.Contracts.IScriptContainerRepository _scriptContainerRepository;
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;

        // public AddDatabaseViewModel() { }//Designer use   //removed because for somereason IoC is using this ctor instead of the correct one

        public AddDatabaseViewModel(Processor.Data.Contracts.IScriptContainerRepository scriptContainerRepository,
            NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            FileAndFolderDialog.Abstractions.IFileDialogService fileDialogService,
            FileAndFolderDialog.Abstractions.IFolderDialogService folderDialogService,
            DatabaseConnectionControlViewModel databaseConnectionControlVM,
            NLog.ILogger logger)
        : base(navigator, fileDialogService, folderDialogService, databaseConnectionControlVM, logger)
        {
            ViewTitle = "Add Database";
            this._scriptContainerRepository = scriptContainerRepository;
            _navigator = navigator;

        }

        /// <summary>
        /// Executes the Save command 
        /// </summary>
        public override void Save()
        {
            if (!GetValidationResult().IsValid)
            {
                ShowErrors = true;
                return;
            }

            var connectionParams = this.UseDefaultDatabaseConnection ? null : this.DatabaseConnectionControlVM.BuildConnectionParameters();

            var result = _scriptContainerRepository.AddNew(databaseName: this.DatabaseName,
                scriptContainerPath: this.ScriptContainerPath,
                customConnectionParameters: connectionParams,
                tags: this.Tags);

            if (result.WasSuccessful)
                _navigator.CloseDialog(this);
            else
                _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                    vm.Init(title: "Add Failed",
                       message: result.Message,
                       buttons: MessageBoxViewModel.MessageBoxButton.OK,
                       icon: MessageBoxViewModel.MessageBoxImage.Error
                       ));
        }
    }
}
