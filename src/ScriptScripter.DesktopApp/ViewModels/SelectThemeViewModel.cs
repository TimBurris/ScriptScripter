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
using System.Collections.ObjectModel;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class SelectThemeViewModel : ScriptScripterViewModelBase
    {

        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;
        private readonly Processor.Data.Contracts.IConfigurationRepository _configurationRepository;
        private readonly Contracts.IViewModelFaultlessService _viewModelFaultlessService;
        private readonly Contracts.IThemeService _themeService;

        //public SelectThemeViewModel() { }//designer only   //removed because for somereason IoC is using this ctor instead of the correct one
        public SelectThemeViewModel(
            NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            Processor.Data.Contracts.IConfigurationRepository configurationRepository,
            Contracts.IViewModelFaultlessService viewModelFaultlessService,
            Contracts.IThemeService themeService,
            NLog.ILogger logger)
            : base(logger)
        {
            ViewTitle = "Select Theme";
            this._navigator = navigator;
            this._configurationRepository = configurationRepository;
            this._viewModelFaultlessService = viewModelFaultlessService;
            this._themeService = themeService;
        }

        protected override async Task<bool> OnReloadDataAsync(CancellationToken cancellationToken)
        {
            var result = await _viewModelFaultlessService
                                   .TryExecuteSyncAsAsync(() => _themeService.GetAllThemes());

            if (!result.WasSuccessful)
                return false;

            this.ThemeOptions = new ObservableCollection<Themes.ThemeOption>(result.ReturnValue);

            return true;
        }

        #region Binding props and Lists

        public ObservableCollection<Themes.ThemeOption> ThemeOptions
        {
            get { return GetField<ObservableCollection<Themes.ThemeOption>>(); }
            set { SetField(value); }
        }

        #endregion


        #region SelectTheme Command

        private RelayCommand<Themes.ThemeOption> _selectThemeCommand;
        public RelayCommand<Themes.ThemeOption> SelectThemeCommand
        {
            get
            {
                if (_selectThemeCommand == null)
                    _selectThemeCommand = new RelayCommand<Themes.ThemeOption>((param) => this.SelectTheme(param), (param) => this.CanSelectTheme(param));
                return _selectThemeCommand;
            }
        }

        public bool CanSelectTheme(Themes.ThemeOption value)
        {
            return true;
        }

        /// <summary>
        /// Executes the SelectTheme command 
        /// </summary>
        public void SelectTheme(Themes.ThemeOption value)
        {
            var result = _viewModelFaultlessService
                .TryExecute(() => _themeService.ApplyTheme(name: value.Name));


            if (result.WasSuccessful)
                _viewModelFaultlessService
                    .TryExecute(() => _configurationRepository.SetThemeName(value.Name));
        }

        #endregion

        #region Close Command

        private RelayCommand _closeCommand;

        public RelayCommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand((param) => this.Close(), (param) => this.CanClose());
                return _closeCommand;
            }
        }

        public bool CanClose()
        {
            return true;
        }

        /// <summary>
        /// Executes the Close command 
        /// </summary>
        public void Close()
        {
            _navigator.CloseDialog(this);
        }

        #endregion

        #region Designer data
        protected override void OnLoadDesignData()
        {
            this.ThemeOptions = new ObservableCollection<Themes.ThemeOption>();
            this.ThemeOptions.Add(new Themes.ThemeOption() { Name = "red", ColorBrush = System.Windows.Media.Brushes.Red });
            this.ThemeOptions.Add(new Themes.ThemeOption() { Name = "aqua", ColorBrush = System.Windows.Media.Brushes.Aqua });
            this.ThemeOptions.Add(new Themes.ThemeOption() { Name = "BlueViolet", ColorBrush = System.Windows.Media.Brushes.BlueViolet });
        }

        #endregion
    }
}
