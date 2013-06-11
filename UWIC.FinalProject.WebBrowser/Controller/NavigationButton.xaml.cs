using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UWIC.FinalProject.WebBrowser.ViewModel;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for NavigationButton.xaml
    /// </summary>
    public partial class NavigationButton : Button
    {
        public NavigationButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The image displayed by the button.
        /// </summary>
        /// <remarks>The image is specified in XAML as an absolute or relative path.</remarks>
        [Description("The image displayed by the button"), Category("Common Properties")]
        public ImageSource DefaultNavigationImage
        {
            get { return (ImageSource)GetValue(ImageProperty1); }
            set { SetValue(ImageProperty1, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty ImageProperty1 = DependencyProperty.Register("DefaultNavigationImage", typeof(ImageSource), typeof(BookmarkButton), new UIPropertyMetadata(null));

        ///// <summary>
        ///// The text displayed by the button.
        ///// </summary>
        [Description("The hover image displayed by the button."), Category("Common Properties")]
        public ImageSource HoverNavigateImage
        {
            get { return (ImageSource)GetValue(ImageProperty2); }
            set { SetValue(ImageProperty2, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty ImageProperty2 = DependencyProperty.Register("HoverNavigateImage", typeof(ImageSource), typeof(BookmarkButton), new UIPropertyMetadata(null));

        [Description("The hover image displayed by the button."), Category("Common Properties")]
        public CommandType CommandType
        {
            get { return (CommandType)GetValue(_CommandType); }
            set { SetValue(_CommandType, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty _CommandType = DependencyProperty.Register("CommandType", typeof(CommandType), typeof(BookmarkButton), new UIPropertyMetadata(null));

        public static BrowserContainerViewModel _browserContainerViewModel;

        public ICommand _functionCommand;
        public ICommand FunctionCommand
        {
            get
            {
                if (_functionCommand == null)
                {
                    _functionCommand = new RelayCommand(ExecuteFunction);
                }
                return _functionCommand;
            }
        }

        public static void SetBrowserContainerViewModel(BrowserContainerViewModel bcViewModel)
        {
            _browserContainerViewModel = bcViewModel;
        }

        private void ExecuteFunction()
        {
            if (CommandType == Controller.CommandType.Forward)
                _browserContainerViewModel.MoveForward();
            else if (CommandType == Controller.CommandType.Backward)
                _browserContainerViewModel.MoveBackward();
            else if (CommandType == Controller.CommandType.Refresh)
                _browserContainerViewModel.RefreshBrowserWindow();
            else if (CommandType == Controller.CommandType.Stop)
                _browserContainerViewModel.StopBrowser();
            else if (CommandType == Controller.CommandType.Go)
                _browserContainerViewModel.NavigateToURL();
        }
    }

    public enum CommandType
    {
        Go,
        Forward,
        Backward,
        Refresh,
        Stop
    }
}
