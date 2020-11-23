using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaMvvm;
using NinjaMvvm.Wpf;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class MessageBoxViewModel : ScriptScripterViewModelBase
    {
        public enum MessageBoxResult
        {
            //
            // Summary:
            //     The message box returns no result.
            None = 0,
            //
            // Summary:
            //     The result value of the message box is OK.
            OK = 1,
            //
            // Summary:
            //     The result value of the message box is Cancel.
            Cancel = 2,
            //
            // Summary:
            //     The result value of the message box is Yes.
            Yes = 6,
            //
            // Summary:
            //     The result value of the message box is No.
            No = 7
        }

        //
        // Summary:
        //     Specifies the buttons that are displayed on a message box. Used as an argument
        [Flags]
        public enum MessageBoxButton
        {
            //
            // Summary:
            //     The message box displays an OK button.
            OK = 1,
            Cancel = 2,
            Yes = 4,
            No = 8,
            //
            // Summary:
            //     The message box displays OK and Cancel buttons.
            OKCancel = 3,
            //
            // Summary:
            //     The message box displays Yes, No, and Cancel buttons.
            YesNoCancel = 14,
            //
            // Summary:
            //     The message box displays Yes and No buttons.
            YesNo = 12
        }

        //
        // Summary:
        //     Specifies constants defining the default button on a Forms.MessageBox.
        public enum MessageBoxDefaultButton
        {
            NoDefault = 0,
            OK = MessageBoxButton.OK,
            Cancel = MessageBoxButton.Cancel,
            Yes = MessageBoxButton.Yes,
            No = MessageBoxButton.No,
        }

        //
        // Summary:
        //     Specifies the icon that is displayed by a message box.
        public enum MessageBoxImage
        {
            //
            // Summary:
            //     No icon is displayed.
            None = 0,
            //
            // Summary:
            //     The message box contains a symbol consisting of a white X in a circle with a
            //     red background.
            Hand = 16,
            //
            // Summary:
            //     The message box contains a symbol consisting of white X in a circle with a red
            //     background.
            Stop = 16,
            //
            // Summary:
            //     The message box contains a symbol consisting of white X in a circle with a red
            //     background.
            Error = 16,
            //
            // Summary:
            //     The message box contains a symbol consisting of a question mark in a circle.
            Question = 32,
            //
            // Summary:
            //     The message box contains a symbol consisting of an exclamation point in a triangle
            //     with a yellow background.
            Exclamation = 48,
            //
            // Summary:
            //     The message box contains a symbol consisting of an exclamation point in a triangle
            //     with a yellow background.
            Warning = 48,
            //
            // Summary:
            //     The message box contains a symbol consisting of a lowercase letter i in a circle.
            Asterisk = 64,
            //
            // Summary:
            //     The message box contains a symbol consisting of a lowercase letter i in a circle.
            Information = 64
        }


        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;


        //public MessageBoxViewModel() { }//designer only   //removed because for somereason IoC is using this ctor instead of the correct one

        public MessageBoxViewModel(NinjaMvvm.Wpf.Abstractions.INavigator navigator,
            NLog.ILogger logger)
            : base(logger)
        {
            this._navigator = navigator;
        }

        public void Init(String title, String message, MessageBoxButton buttons)
        {
            this.ViewTitle = title;
            this.Message = message;
            this.SetButtons(buttons);
        }

        public void Init(String title, String message, MessageBoxButton buttons, MessageBoxImage icon)
        {
            this.ViewTitle = title;
            this.Message = message;
            this.Icon = icon;
            this.SetButtons(buttons);
        }

        public void Init(String title, String message, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxDefaultButton defaultButton)
        {
            this.ViewTitle = title;
            this.Message = message;
            this.Icon = icon;
            this.DefaultButton = defaultButton;
            this.SetButtons(buttons);
        }

        public void SetButtons(MessageBoxButton buttons)
        {
            this.CanCancel = (buttons & MessageBoxButton.Cancel) == MessageBoxButton.Cancel;
            this.CanYes = (buttons & MessageBoxButton.Yes) == MessageBoxButton.Yes;
            this.CanNo = (buttons & MessageBoxButton.No) == MessageBoxButton.No;
            this.CanOK = (buttons & MessageBoxButton.OK) == MessageBoxButton.OK;
        }
        public MessageBoxResult ViewResult { get; private set; }

        public MessageBoxDefaultButton DefaultButton
        {
            get { return GetField<MessageBoxDefaultButton>(); }
            set { SetField(value); }
        }

        protected override void OnLoadDesignData()
        {
            this.ViewTitle = "Lorem ipsum";
            this.Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco";
            this.Icon = MessageBoxImage.Question;

            this.CanShowMoreDetails = true;
            this.MoreDetailsCaption = "Show details";

            this.DisplayMoreDetails = false;
            this.MoreDetailsMessage = @"Lorem ipsum dolor sit amet, mei vitae tacimates id, justo clita appetere te has, eu nam probo facilisi. Justo minim epicurei usu eu, id harum expetendis mei. Eu est numquam ceteros hendrerit, eius simul omnium pri id, nec habemus apeirian ex. Ex ferri regione erroribus mea, nec id quem dolorum qualisque. Postea petentium dissentiet duo ne.

Fastidii placerat delicata mea et, quo vocent appareat et. Nam ut verterem salutandi patrioque, facilis deterruisset quo ne. Augue aliquip patrioque eu quo, ad ius amet nostro, cu eius deserunt ius.Vel erat antiopam ne, vis semper efficiantur cu.Tation sensibus suavitate usu ad.

Eam vidit aliquip reprimique cu, ne fabellas splendide vix. Hinc reque tamquam in vix, et vel summo legimus, vidit explicari reformidans in nec.Ne liber urbanitas concludaturque eum, ne modus concludaturque quo, nam ei saperet eloquentiam.In quo decore accommodare. Eius placerat gubergren nec at, te qui modus clita. Ad pri zril atomorum.

At has graeci propriae inciderint, has dicta noster dolorem et.Percipit iracundia ei per, mel id veri fugit.Cu facilis elaboraret pro, vix et alii nisl quaeque. Per fugit tantas iudicabit an.Sensibus definiebas pri cu, ei verear propriae mnesarchum nec. Vel dicant debitis urbanitas et.

Mei adhuc movet imperdiet cu.Te meliore blandit sea. Est ei quaeque antiopam temporibus, nisl disputationi cu pro. Vix dolores quaestio ne. At per exerci imperdiet, ne eum dignissim adolescens.";

            this.CanCancel = true;
            this.CanYes = true;
            this.CanNo = true;
            this.CanOK = true;
        }

        private void Close()
        {
            _navigator.CloseDialog(this);
        }

        #region binding properties
        public string Message
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        private MessageBoxImage _icon;
        public MessageBoxImage Icon
        {
            get { return GetField<MessageBoxImage>(); }
            set
            {
                if (SetField(value))
                {
                    //assign the icon source
                    this.AssignIconSource();
                }
            }
        }

        public System.Windows.Media.Imaging.BitmapSource IconSource
        {
            get { return GetField<System.Windows.Media.Imaging.BitmapSource>(); }
            set { SetField(value); }
        }

        public bool CanCancel
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public bool CanYes
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }
        public bool CanNo
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }
        public bool CanOK
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        public bool DisplayMoreDetails
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }
        public string MoreDetailsCaption
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public string MoreDetailsMessage
        {
            get { return GetField<string>(); }
            set { SetField(value); }
        }

        public bool CanShowMoreDetails
        {
            get { return GetField<bool>(); }
            set { SetField(value); }
        }

        #endregion

        #region Commands

        #region Yes Relay Command
        private RelayCommand _yesCommand;
        public RelayCommand YesCommand
        {
            get
            {
                if (_yesCommand == null)
                    _yesCommand = new RelayCommand((param) => this.Yes());
                return _yesCommand;
            }
        }

        /// <summary>
        /// Executes the Yes command 
        /// </summary>
        public void Yes()
        {
            this.ViewResult = MessageBoxResult.Yes;
            this.Close();
        }
        #endregion

        #region No Relay Command
        private RelayCommand _noCommand;
        public RelayCommand NoCommand
        {
            get
            {
                if (_noCommand == null)
                    _noCommand = new RelayCommand((param) => this.No());
                return _noCommand;
            }
        }

        /// <summary>
        /// Executes the No command 
        /// </summary>
        public void No()
        {
            this.ViewResult = MessageBoxResult.No;
            this.Close();
        }
        #endregion

        #region OK Relay Command
        private RelayCommand _okCommand;
        public RelayCommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                    _okCommand = new RelayCommand((param) => this.OK());
                return _okCommand;
            }
        }

        /// <summary>
        /// Executes the OK command 
        /// </summary>
        public void OK()
        {
            this.ViewResult = MessageBoxResult.OK;
            this.Close();
        }
        #endregion

        #region Cancel Relay Command
        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new RelayCommand((param) => this.Cancel());
                return _cancelCommand;
            }
        }

        /// <summary>
        /// Executes the cancel command 
        /// </summary>
        public void Cancel()
        {
            this.ViewResult = MessageBoxResult.Cancel;
            this.Close();
        }
        #endregion

        #region ShowMoreDetails Relay Command
        private RelayCommand _showMoreDetailsCommand;

        public RelayCommand ShowMoreDetailsCommand
        {
            get
            {
                if (_showMoreDetailsCommand == null)
                    _showMoreDetailsCommand = new RelayCommand((param) => this.ShowMoreDetails());
                return _showMoreDetailsCommand;
            }
        }

        /// <summary>
        /// Executes the MoreDetails command 
        /// </summary>
        public void ShowMoreDetails()
        {
            this.CanShowMoreDetails = false;
            this.DisplayMoreDetails = true;
        }
        #endregion

        #endregion

        private void AssignIconSource()
        {
            System.Windows.Media.Imaging.BitmapSource bs;

            if (this.Icon == MessageBoxImage.None)
                bs = null;
            else
            {
                System.Drawing.Icon image = (System.Drawing.Icon)typeof(System.Drawing.SystemIcons).GetProperty(this.Icon.ToString(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null, null);
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(image.Handle, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            this.IconSource = bs;
        }

    }
}
