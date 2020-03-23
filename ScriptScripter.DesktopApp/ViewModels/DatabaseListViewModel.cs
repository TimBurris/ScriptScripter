using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NinjaMvvm;
using NinjaMvvm.Wpf;
using FaultlessExecution.Extensions;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class DatabaseListViewModel : ScriptScripterViewModelBase
    {
        private readonly System.IO.Abstractions.IFileSystem _fileSystem;
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private readonly Processor.Data.Contracts.IScriptRepositoryFactory _scriptsRepoFactory;
        private readonly Processor.Data.Contracts.IScriptContainerRepository _scriptsContainerRepository;
        private readonly Processor.Services.Contracts.IEventNotificationService _eventNotificationService;
        private List<ViewModels.DatabaseListViewModel.LineItem> _allLineItems = new List<LineItem>();

        public DatabaseListViewModel() { }//Designer only

        public DatabaseListViewModel(System.IO.Abstractions.IFileSystem fileSystem,
            NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Processor.Data.Contracts.IScriptRepositoryFactory scriptsRepoFactory,
            Processor.Data.Contracts.IScriptContainerRepository scriptsContainerRepository,
            Processor.Services.Contracts.IEventNotificationService eventNotificationService)
        {
            this._fileSystem = fileSystem;
            this._navigator = navigator;
            this._viewModelFaultlessService = viewModelFaultlessService;
            this._scriptsRepoFactory = scriptsRepoFactory;
            this._scriptsContainerRepository = scriptsContainerRepository;
            this._eventNotificationService = eventNotificationService;
            this.Tags.ItemPropertyChangedEvent += Tags_ItemPropertyChangedEvent;

            //TODO: confirm these are not hanging around after dead
            _eventNotificationService.ScriptContainerAdded -= _eventNotificationService_ContainersChange;
            _eventNotificationService.ScriptContainerUpdated -= _eventNotificationService_ContainersChange;
            _eventNotificationService.ScriptContainerRemoved -= _eventNotificationService_ContainersChange;
            _eventNotificationService.ScriptContainerContentsChanged -= _eventNotificationService_ContainersChange;
        }

        private void Tags_ItemPropertyChangedEvent(object sender, ItemPropertyChangedEventArgs<TagLineItem> e)
        {
            this.FilterLineItems();
        }

       private void _eventNotificationService_ContainersChange(object sender, EventArgs e)
        {
            this.ReloadDataAsync();
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            //TODO: store selected and restore
            this.Tags.Clear();
            _allLineItems.Clear();
            LineItems = new System.Collections.ObjectModel.ObservableCollection<ViewModels.DatabaseListViewModel.LineItem>();
            List<string> allTags = new List<string>();

            var scriptContainers = _scriptsContainerRepository.GetAll();

            foreach (var scriptContainer in scriptContainers)
            {
                if (scriptContainer.Tags != null)
                    allTags.AddRange(scriptContainer.Tags);

                await _viewModelFaultlessService.TryExecuteSyncAsAsync(() =>
                   {
                       var repo = _scriptsRepoFactory.GetScriptsRepository(scriptContainer.ScriptFilePath);
                       return repo.GetLastScript();
                   })
                   .OnSuccessAsync(result =>
                   {
                       _allLineItems.Add(this.BuildLineItem(scriptContainer, result.ReturnValue));
                   })
                   .OnExceptionAsync(result =>
                   {
                       var lineItem = this.BuildLineItem(scriptContainer, script: null);
                       lineItem.IsFailed = true;
                       _allLineItems.Add(lineItem);
                   })
                   ;
            }

            allTags
                .Distinct()
                .Select(s => new TagLineItem() { TagName = s })
                .ToList()
                .ForEach(t => this.Tags.Add(t));

            this.FilterLineItems();
            return true;
        }

        private LineItem BuildLineItem(Processor.Data.Models.ScriptContainer scriptContainer,
            Processor.Data.Models.Script script)
        {
            // remember, script can be null, doesn't mean there's an issue, just might not be any scripts in the container

            var item = new ViewModels.DatabaseListViewModel.LineItem
            {
                ScriptContainer = scriptContainer,
                ScriptFile = scriptContainer.ScriptFilePath,
                DatabaseName = scriptContainer.DatabaseName,
                DeveloperName = script?.DeveloperName,
                RevisionNumber = script?.RevisionNumber.ToString(),
                ScriptDate = script?.ScriptDate.ToString(),
                ServerConnectionInfo = this.GetConnectionDisplayText(scriptContainer.CustomServerConnectionParameters),
            };

            if (scriptContainer.Tags != null)
                item.TagNames = string.Join(", ", scriptContainer.Tags);

            return item;
        }

        private void FilterLineItems()
        {
            var selectedTags = this.Tags
                            .Where(t => t.IsSelected)
                            .Select(t => t.TagName)
                            .ToList();

            List<LineItem> items;
            if (selectedTags.Any())
            {
                items = _allLineItems
                    .Where(x => x.ScriptContainer.Tags?.Intersect(selectedTags)?.Any() ?? false)
                    .ToList();
            }
            else
            {
                items = _allLineItems.ToList();
            }

            this.LineItems = new System.Collections.ObjectModel.ObservableCollection<LineItem>(items);
        }

        private string GetConnectionDisplayText(Processor.Data.Models.ServerConnectionParameters connectionInfo)
        {
            if (connectionInfo == null)
                return null;

            return connectionInfo.ToString();
        }

        protected override void OnLoadDesignData()
        {
            Tags.Add(new TagLineItem()
            {
                IsSelected = true,
                TagName = "Project A"
            });
            Tags.Add(new TagLineItem()
            {
                IsSelected = false,
                TagName = "Project B"
            });
            Tags.Add(new TagLineItem()
            {
                IsSelected = true,
                TagName = "Test"
            });
            Tags.Add(new TagLineItem()
            {
                IsSelected = false,
                TagName = "Production"
            });

            LineItems = new System.Collections.ObjectModel.ObservableCollection<ViewModels.DatabaseListViewModel.LineItem>();
            LineItems.Add(new ViewModels.DatabaseListViewModel.LineItem
            {
                ScriptFile = @"C:\Code\MyProject\Database\Scripts\DBScripts_SampleDatabase.xml",
                DatabaseName = "SampleData",
                DeveloperName = "Cpt. Jack Sparrow",
                RevisionNumber = "97",
                ScriptDate = DateTime.Now.AddHours(-5498797).ToString(),
            });
            LineItems.Add(new ViewModels.DatabaseListViewModel.LineItem
            {
                ScriptFile = @"C:\Microsoft\Database\DBScripts_MailDB.xml",
                DatabaseName = "MailDB",
                DeveloperName = "Benny Jet",
                RevisionNumber = "16",
                ScriptDate = DateTime.Now.AddHours(-54654).ToString(),
                ServerConnectionInfo = "(local) Integrated Security",
                TagNames = "Project A"
            });
            LineItems.Add(new ViewModels.DatabaseListViewModel.LineItem
            {
                ScriptFile = @"C:\Microsoft\Database\DBScripts_Northwind.xml",
                DatabaseName = "Northwind",
                DeveloperName = "Dumpster Ninja",
                RevisionNumber = "2",
                ScriptDate = DateTime.Now.AddHours(-4545767).ToString(),
            });
            LineItems.Add(new ViewModels.DatabaseListViewModel.LineItem
            {
                ScriptFile = @"C:\Code\YourProject\Myproject\Database\Scripts\DBScripts_DatbaseX.xml",
                DatabaseName = "DatbaseX",
                DeveloperName = "Cpt. Jack Sparrow",
                RevisionNumber = "978",
                ScriptDate = DateTime.Now.AddHours(-345).ToString(),
                ServerConnectionInfo = "Pizza\\ProdSQL sa ********",
                TagNames = "Project A, Test, Production"
            });
            LineItems.Add(new ViewModels.DatabaseListViewModel.LineItem
            {
                ScriptFile = @"C:\Code\YourProject\Myproject\Database\Scripts\bad_data.xml",
                DatabaseName = "DatbaseX",
                IsFailed = true,
            });
        }

        public System.Collections.ObjectModel.ObservableCollection<LineItem> LineItems
        {
            get { return GetField<System.Collections.ObjectModel.ObservableCollection<LineItem>>(); }
            set { SetField(value); }
        }


        public NotificationObservableCollection<TagLineItem> Tags { get; }
                    = new NotificationObservableCollection<TagLineItem>();

        #region OpenFolder Command

        private RelayCommand<LineItem> _openFolderCommand;
        public RelayCommand<LineItem> OpenFolderCommand
        {
            get
            {
                if (_openFolderCommand == null)
                    _openFolderCommand = new RelayCommand<LineItem>((param) => this.OpenFolder(param), (param) => this.CanOpenFolder(param));
                return _openFolderCommand;
            }
        }

        public bool CanOpenFolder(LineItem lineItem)
        {
            return true;
        }

        /// <summary>
        /// Executes the OpenFolder command 
        /// </summary>
        public void OpenFolder(LineItem lineItem)
        {
            var folder = _fileSystem.FileInfo.FromFileName(lineItem.ScriptFile)
                .Directory;

            if (folder.Exists)
                System.Diagnostics.Process.Start(folder.FullName);
            else
            {
                _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                    vm.Init(title: "Folder Not Found",
                        message: "The containing folder does not exist",
                        buttons: MessageBoxViewModel.MessageBoxButton.OK,
                        icon: MessageBoxViewModel.MessageBoxImage.Error)
                );
            }
        }

        #endregion

        #region Open Command

        private RelayCommand<LineItem> _openCommand;
        public RelayCommand<LineItem> OpenCommand
        {
            get
            {
                if (_openCommand == null)
                    _openCommand = new RelayCommand<LineItem>((param) => this.Open(param), (param) => this.CanOpen());
                return _openCommand;
            }
        }

        public bool CanOpen()
        {
            return true;
        }

        /// <summary>
        /// Executes the Open command 
        /// </summary>
        public void Open(LineItem lineItem)
        {
            _navigator.NavigateTo<DatabaseScriptsViewModel>(vm => vm.Init(scriptContainer: lineItem.ScriptContainer));
        }

        #endregion

        #region Remove Command

        private RelayCommand<LineItem> _removeCommand;
        public RelayCommand<LineItem> RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                    _removeCommand = new RelayCommand<LineItem>((param) => this.Remove(param), (param) => this.CanRemove());
                return _removeCommand;
            }
        }

        public bool CanRemove()
        {
            return true;
        }

        /// <summary>
        /// Executes the Remove command 
        /// </summary>
        public void Remove(LineItem lineItem)
        {

            var confirmViewModel = this._navigator.ShowDialog<MessageBoxViewModel>(vm =>
              {
                  vm.ViewTitle = "Confirm Remove";
                  vm.Message = "Are you sure you would like to remove this database from the list?\r\n(Note= the script file itself will be unaffected)";
                  vm.SetButtons(MessageBoxViewModel.MessageBoxButton.YesNo);
                  vm.Icon = MessageBoxViewModel.MessageBoxImage.Question;
                  vm.DefaultButton = MessageBoxViewModel.MessageBoxDefaultButton.No;
              });

            if (confirmViewModel.ViewResult == MessageBoxViewModel.MessageBoxResult.Yes)
            {

                var result = _scriptsContainerRepository.Remove(lineItem.ScriptContainer.ContainerUid);
                if (!result.WasSuccessful)
                {
                    _navigator.ShowDialog<MessageBoxViewModel>(vm =>
                       {
                           vm.ViewTitle = "Remove Failed";
                           vm.Message = result.Message;
                           vm.SetButtons(MessageBoxViewModel.MessageBoxButton.OK);
                           vm.Icon = MessageBoxViewModel.MessageBoxImage.Error;
                       });
                }
            }
        }

        #endregion

        #region Edit Command

        private RelayCommand<LineItem> _editCommand;
        public RelayCommand<LineItem> EditCommand
        {
            get
            {
                if (_editCommand == null)
                    _editCommand = new RelayCommand<LineItem>((param) => this.Edit(param), (param) => this.CanEdit());
                return _editCommand;
            }
        }

        public bool CanEdit()
        {
            return true;
        }

        /// <summary>
        /// Executes the Edit command 
        /// </summary>
        public void Edit(LineItem lineItem)
        {
            _navigator.ShowDialog<EditDatabaseViewModel>(vm => vm.Init(containerUid: lineItem.ScriptContainer.ContainerUid));
        }

        #endregion

        #region Add Command

        private RelayCommand _addCommand;

        public RelayCommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                    _addCommand = new RelayCommand((param) => this.Add(), (param) => this.CanAdd());
                return _addCommand;
            }
        }

        public bool CanAdd()
        {
            return true;
        }

        /// <summary>
        /// Executes the Add command 
        /// </summary>
        public void Add()
        {
            _navigator.ShowDialog<AddDatabaseViewModel>();
        }

        #endregion

        #region LineItem

        public class LineItem : NotificationBase
        {
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

            public string RevisionNumber
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public string ScriptDate
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public string DeveloperName
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public Processor.Data.Models.ScriptContainer ScriptContainer
            {
                get { return GetField<Processor.Data.Models.ScriptContainer>(); }
                set { SetField(value); }
            }

            public bool IsFailed
            {
                get { return GetField<bool>(); }
                set { SetField(value); }
            }

            public string TagNames
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public string ServerConnectionInfo
            {
                get { return GetField<string>(); }
                set
                {
                    if (SetField(value))
                        OnPropertyChanged(nameof(HasServerConnectionInfo));
                }
            }

            public bool HasServerConnectionInfo { get { return !string.IsNullOrEmpty(ServerConnectionInfo); } }
        }

        #endregion

        public class TagLineItem : NotificationBase
        {

            public string TagName
            {
                get { return GetField<string>(); }
                set { SetField(value); }
            }

            public bool IsSelected
            {
                get { return GetField<bool>(); }
                set { SetField(value); }
            }

        }
    }
}
