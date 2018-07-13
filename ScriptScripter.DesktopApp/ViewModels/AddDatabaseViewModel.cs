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

        public AddDatabaseViewModel()
        {
            ViewTitle = "Add Database";
        }

        public AddDatabaseViewModel(string defaultFileNamePattern)
            : base(defaultFileNamePattern)
        {
            ViewTitle = "Add Database";
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

            var result = this.ScriptContainerRepository.AddNew(databaseName: this.DatabaseName,
                scriptFilePath: this.ScriptFile,
                customConnectionParameters: connectionParams,
                tags: this.Tags);

            if (result.WasSuccessful)
                Navigator.CloseDialog(this);
            else
                Navigator.ShowDialog<MessageBoxViewModel>(vm =>
                    vm.Init(title: "Add Failed",
                       message: result.Message,
                       buttons: MessageBoxViewModel.MessageBoxButton.OK,
                       icon: MessageBoxViewModel.MessageBoxImage.Error
                       ));
        }
    }
}
