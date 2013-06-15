using System;
using System.Collections.Generic;
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
    /// Interaction logic for TabItemHeader.xaml
    /// </summary>
    public partial class TabItemHeader : UserControl
    {
        private static TabItemViewModel _ViewModel { get; set; }
        public CharacterCasing Casing = CharacterCasing.Upper;

        public TabItemHeader(BitmapImage image, string title)
        {
            InitializeComponent();
            pageIcon.Source = image;
            PageTitle.Text = title;
            ToolTipService.SetToolTip(PageTitle, PageTitle.Text);
            this.Resources.Add("ConvertUpperCase", Casing);
        }

        private void btnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var parent = (UIElement)this.Parent;
            var _parent = (TabItem)parent;
            int hashCode = _parent.GetHashCode();
            _ViewModel.RemoveTabItem(hashCode);
        }

        public static void setViewModel(TabItemViewModel vm)
        {
            _ViewModel = vm;
        }
    }
}
