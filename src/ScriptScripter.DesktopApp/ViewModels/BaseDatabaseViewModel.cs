﻿using NinjaMvvm.Wpf;
using FluentValidation;
using System.Linq;
using System.Collections.ObjectModel;
using FileAndFolderDialog.Abstractions;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public abstract class BaseDatabaseViewModel : ScriptScripterViewModelBase
    {
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly FileAndFolderDialog.Abstractions.IFileDialogService _fileDialogService;
        private readonly IFolderDialogService _folderDialogService;

        //public BaseDatabaseViewModel() { }//designer use   //removed because for somereason IoC is using this ctor instead of the correct one

        public BaseDatabaseViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            FileAndFolderDialog.Abstractions.IFileDialogService fileDialogService,
            FileAndFolderDialog.Abstractions.IFolderDialogService folderDialogService,
            DatabaseConnectionControlViewModel databaseConnectionControlVM,
            NLog.ILogger logger)
            : base(logger)
        {
            UseDefaultDatabaseConnection = true;
            this.Tags = new ObservableCollection<string>();
            this._navigator = navigator;
            this._fileDialogService = fileDialogService;
            _folderDialogService = folderDialogService;
            this.DatabaseConnectionControlVM = databaseConnectionControlVM;
            this.DefaultFileNamePattern = ScriptScripter.DesktopApp.Properties.Settings.Default.NewFileDefaultNameFormat;
        }

        public DatabaseConnectionControlViewModel DatabaseConnectionControlVM { get; }

        #region Binding props and Lists

        public string DefaultFileNamePattern
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

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

        public string ScriptContainerPath
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
            _navigator.CloseDialog(this);
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
                defaultFileName = this.DefaultFileNamePattern;
                defaultFileName = System.Text.RegularExpressions.Regex.Replace(
                    input: defaultFileName,
                    pattern: "{DatabaseName}",
                    replacement: this.DatabaseName,
                    options: System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            var result = _fileDialogService.ShowSelectFileDialog(new FileAndFolderDialog.Abstractions.OpenFileOptions()
            {
                AddExtension = true,
                CheckFileExists = false,
                Filter = "Xml Files | *.xml",
                DefaultFileName = defaultFileName,
            })
            ?.SingleOrDefault();

            this.ScriptContainerPath = result ?? this.ScriptContainerPath;
        }

        #endregion

        #region SelectFolder Command

        private RelayCommand _selectFolderCommand;

        public RelayCommand SelectFolderCommand
        {
            get
            {
                if (_selectFolderCommand == null)
                    _selectFolderCommand = new RelayCommand((param) => this.SelectFolder(), (param) => this.CanSelectFolder());
                return _selectFolderCommand;
            }
        }

        public bool CanSelectFolder()
        {
            return true;
        }

        /// <summary>
        /// Executes the SelectFolder command 
        /// </summary>
        public void SelectFolder()
        {
            var result = _folderDialogService.ShowSelectFolderDialog(new SelectFolderOptions()
            {
                RootFolder = System.Environment.SpecialFolder.MyComputer,
                SelectedPath = this.ScriptContainerPath,
                ShowNewFolderButton = true,
            });

            this.ScriptContainerPath = result ?? this.ScriptContainerPath;
        }

        #endregion

        #region Validation

        class BaseDatabaseViewModelValidator : AbstractValidator<BaseDatabaseViewModel>
        {
            public BaseDatabaseViewModelValidator()
            {
                RuleFor(obj => obj.DatabaseName).NotEmpty();
                RuleFor(obj => obj.ScriptContainerPath).NotEmpty();
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
            ScriptContainerPath = @"C:\Code\MyProject\Database\Scripts\DBScripts_SampleDatabase.xml";
            UseDefaultDatabaseConnection = false;
        }

        #endregion

    }
}
