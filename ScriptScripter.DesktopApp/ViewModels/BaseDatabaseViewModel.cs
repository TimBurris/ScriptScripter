using NinjaMvvm.Wpf;
using FluentValidation;
using System.Linq;
using System.Collections.ObjectModel;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public abstract class BaseDatabaseViewModel : ScriptScripterViewModelBase
    {
        private readonly string _defaultFileNamePattern;

        [Ninject.Inject]
        public Processor.Data.Contracts.IScriptContainerRepository ScriptContainerRepository { get; set; }

        [Ninject.Inject]
        public FileAndFolderDialog.Abstractions.IFileDialogService FileDialogService { get; set; }
        [Ninject.Inject]
        public DatabaseConnectionControlViewModel DatabaseConnectionControlVM { get; set; }

        public BaseDatabaseViewModel()
        {
            _defaultFileNamePattern = ScriptScripter.DesktopApp.Properties.Settings.Default.NewFileDefaultNameFormat;
            UseDefaultDatabaseConnection = true;
            this.Tags = new ObservableCollection<string>();
        }

        public BaseDatabaseViewModel(string defaultFileNamePattern)
            : this()
        {
            this._defaultFileNamePattern = defaultFileNamePattern;
        }

        #region Binding props and Lists

        public ObservableCollection<string> Tags
        {
            get { return GetField<ObservableCollection<string>>(); }
            set { SetField(value); }
        }


        public bool UseDefaultDatabaseConnection
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public string DatabaseName
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string ScriptFile
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
        public abstract void Save();

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

        #region SelectFile Command

        private RelayCommand _selectFileCommand;

        public RelayCommand SelectFileCommand
        {
            get
            {
                if (_selectFileCommand == null)
                    _selectFileCommand = new RelayCommand((param) => this.SelectFile(), (param) => this.CanSelectFile());
                return _selectFileCommand;
            }
        }

        public bool CanSelectFile()
        {
            return true;
        }

        /// <summary>
        /// Executes the SelectFile command 
        /// </summary>
        public void SelectFile()
        {
            string defaultFileName = null;

            if (!string.IsNullOrEmpty(this.DatabaseName))
            {
                defaultFileName = _defaultFileNamePattern;
                defaultFileName = System.Text.RegularExpressions.Regex.Replace(
                    input: defaultFileName,
                    pattern: "{DatabaseName}",
                    replacement: this.DatabaseName,
                    options: System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            var result = this.FileDialogService.ShowSelectFileDialog(new FileAndFolderDialog.Abstractions.OpenFileOptions()
            {
                AddExtension = true,
                CheckFileExists = false,
                Filter = "Xml Files | *.xml",
                DefaultFileName = defaultFileName,
            })
            ?.SingleOrDefault();

            this.ScriptFile = result ?? this.ScriptFile;
        }

        #endregion

        #region Validation

        class BaseDatabaseViewModelValidator : AbstractValidator<BaseDatabaseViewModel>
        {
            public BaseDatabaseViewModelValidator()
            {
                RuleFor(obj => obj.DatabaseName).NotEmpty();
                RuleFor(obj => obj.ScriptFile).NotEmpty();
                RuleFor(obj => obj.DatabaseConnectionControlVM.ServerName)
                    .NotEmpty()
                    .When(obj => !obj.UseDefaultDatabaseConnection);
            }
        }

        protected override IValidator GetValidator()
        {
            return new BaseDatabaseViewModelValidator();
        }

        #endregion

        #region Designer data
        protected override void OnLoadDesignData()
        {
            DatabaseName = "SampleDatabase";
            ScriptFile = @"C:\Code\MyProject\Database\Scripts\DBScripts_SampleDatabase.xml";
            UseDefaultDatabaseConnection = false;
        }

        #endregion

    }
}
