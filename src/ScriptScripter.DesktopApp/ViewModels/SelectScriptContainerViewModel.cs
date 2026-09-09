using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaMvvm;
using NinjaMvvm.Wpf;
using System.Collections.ObjectModel;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class SelectScriptContainerViewModel : ScriptScripterViewModelBase
    {
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;

        //public SelectScriptContainerViewModel() { }//designer only   //removed because for somereason IoC is using this ctor instead of the correct one
        public SelectScriptContainerViewModel(
            NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            NLog.ILogger logger)
            : base(logger)
        {
            ViewTitle = "Select Script Container";
            _navigator = navigator;
        }

        public void Init(IEnumerable<Processor.Data.Models.ScriptContainer> containerOptions)
        {
            this.ContainerOptions = new ObservableCollection<Processor.Data.Models.ScriptContainer>(containerOptions);
        }

        /// <summary>
        /// The container the user picked, or null if the dialog was cancelled.
        /// </summary>
        public Processor.Data.Models.ScriptContainer SelectedContainer { get; private set; }

        #region Binding props and Lists

        public ObservableCollection<Processor.Data.Models.ScriptContainer> ContainerOptions
        {
            get { return GetField<ObservableCollection<Processor.Data.Models.ScriptContainer>>(); }
            set { SetField(value); }
        }

        #endregion

        #region SelectContainer Command

        private RelayCommand<Processor.Data.Models.ScriptContainer> _selectContainerCommand;
        public RelayCommand<Processor.Data.Models.ScriptContainer> SelectContainerCommand
        {
            get
            {
                if (_selectContainerCommand == null)
                    _selectContainerCommand = new RelayCommand<Processor.Data.Models.ScriptContainer>((param) => this.SelectContainer(param), (param) => this.CanSelectContainer(param));
                return _selectContainerCommand;
            }
        }

        public bool CanSelectContainer(Processor.Data.Models.ScriptContainer value)
        {
            return true;
        }

        /// <summary>
        /// Executes the SelectContainer command
        /// </summary>
        public void SelectContainer(Processor.Data.Models.ScriptContainer value)
        {
            this.SelectedContainer = value;
            _navigator.CloseDialog(this);
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
            this.SelectedContainer = null;
            _navigator.CloseDialog(this);
        }

        #endregion

        #region Designer data
        protected override void OnLoadDesignData()
        {
            this.ContainerOptions = new ObservableCollection<Processor.Data.Models.ScriptContainer>();
            this.ContainerOptions.Add(new Processor.Data.Models.ScriptContainer()
            {
                DatabaseName = "SyndeoBroker",
                CustomServerConnectionParameters = new Processor.Data.Models.ServerConnectionParameters() { Server = "(local)", UseTrustedConnection = true },
            });
            this.ContainerOptions.Add(new Processor.Data.Models.ScriptContainer()
            {
                DatabaseName = "SyndeoWaste",
                CustomServerConnectionParameters = new Processor.Data.Models.ServerConnectionParameters() { Server = "(local)", UseTrustedConnection = true },
            });
        }

        #endregion
    }
}
