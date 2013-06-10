using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UWIC.FinalProject.WebBrowser.Controller;
using Elysium;

namespace UWIC.FinalProject.WebBrowser.ViewModel
{
    public class TabItemViewModel : MainViewModel
    {
        # region Commands

        public ICommand ClickCommand { get; set; }

        # endregion

        #region Properties

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        private ObservableCollection<TabItem> _tabitems = new ObservableCollection<TabItem>();
        public ObservableCollection<TabItem> TabItems
        {
            get
            {
                return _tabitems;
            }
            set
            {
                _tabitems = value;
                OnPropertyChanged("TabItems");
            }
        }
        
        #endregion

        public TabItemViewModel()
        {
            TabItem myItem = new TabItem();
            myItem.Header = getNewTabItemHeader();
            myItem.Content = new Controller.BrowserContainer();
            TabItems.Add(myItem);

            this.ClickCommand = new RelayCommand(AddTabItem);
            TabItemHeader.setViewModel(this);
        }

        private TabItemHeader getNewTabItemHeader()
        {
            return new TabItemHeader(getBlankPageImage(), "New Tab");
        }

        public BitmapImage getBlankPageImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("/Images/icon-page.png", UriKind.RelativeOrAbsolute);
            image.EndInit();
            return image;
        }

        /// <summary>
        /// This method is used to add a new tab item to the tab control
        /// </summary>
        public void AddTabItem()
        {
            TabItem myItem = new TabItem();
            myItem.Header = getNewTabItemHeader();
            myItem.Content = new Controller.BrowserContainer();
            TabItems.Add(myItem);
            SelectedIndex = TabItems.Count - 1;
        }

        /// <summary>
        /// This method is used to Remove a particular tabitem by providing the HashCode of it
        /// </summary>
        /// <param name="HashCode">Hash-Code of the tabitem</param>
        public void RemoveTabItem(object HashCode)
        {
            var res = TabItems.First(rec => rec.GetHashCode() == (int)HashCode);
            TabItems.Remove(res);
        }

    }
}
