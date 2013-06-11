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
    /// Interaction logic for BookmarkButton.xaml
    /// </summary>
    public partial class BookmarkButton : UserControl
    {
        public BookmarkButton()
        {
            // Initialize dependency properties
            InitializeComponent();
            //TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TaskButton), new UIPropertyMetadata(null));
        }

        #region Custom Control Properties

        /// <summary>
        /// The image displayed by the button.
        /// </summary>
        /// <remarks>The image is specified in XAML as an absolute or relative path.</remarks>
        [Description("The image displayed by the button"), Category("Common Properties")]
        public ImageSource DefaultImage
        {
            get { return (ImageSource)GetValue(ImageProperty1); }
            set { SetValue(ImageProperty1, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty ImageProperty1 = DependencyProperty.Register("DefaultImage", typeof(ImageSource), typeof(BookmarkButton), new UIPropertyMetadata(null));

        ///// <summary>
        ///// The text displayed by the button.
        ///// </summary>
        [Description("The hover image displayed by the button."), Category("Common Properties")]
        public ImageSource HoverImage
        {
            get { return (ImageSource)GetValue(ImageProperty2); }
            set { SetValue(ImageProperty2, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty ImageProperty2 = DependencyProperty.Register("HoverImage", typeof(ImageSource), typeof(BookmarkButton), new UIPropertyMetadata(null));

        [Description("The Parameter Passed to the command"), Category("Common Properties")]
        public string CommandParam
        {
            get { return (string)GetValue(_commandParam); }
            set { SetValue(_commandParam, value); }

        }
        public static readonly DependencyProperty _commandParam = DependencyProperty.Register("CommandParam", typeof(string), typeof(BookmarkButton), new UIPropertyMetadata(null));

        public ICommand _bookmarkCommand;
        public ICommand BookmarkCommand
        {
            get
            {
                if (_bookmarkCommand == null)
                {
                    _bookmarkCommand = new RelayCommand(param => this.moveToBookmark(param));
                }
                return _bookmarkCommand;
            }
        }

        public static BrowserContainerViewModel _browserContainerVM;

        public static void SetBrowserContainerViewModel(BrowserContainerViewModel bcViewModel)
        {
            _browserContainerVM = bcViewModel;
        }

        private void moveToBookmark(object url)
        {
            Uri URL;
            Uri.TryCreate(url.ToString(), UriKind.RelativeOrAbsolute, out URL);
            _browserContainerVM.NavigateToURL(URL);
        }

        #endregion
    }
}
