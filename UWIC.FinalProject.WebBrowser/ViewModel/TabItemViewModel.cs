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
    public class TabItemViewModel : INotifyPropertyChanged
    {
        # region Commands

        public ICommand ClickCommand { get; set; }
        public ICommand RemoveTabCommand { get; set; }

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

            this.ClickCommand = new RelayCommand(Execute);
            this.RemoveTabCommand = new RelayCommand(RemoveTabItem);
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

        public void Execute()
        {
            TabItem myItem = new TabItem();
            myItem.Header = getNewTabItemHeader();
            myItem.Content = new Controller.BrowserContainer();
            TabItems.Add(myItem);
            SelectedIndex = TabItems.Count - 1;
        }

        public void RemoveTabItem(object HashCode)
        {
            var res = TabItems.First(rec => rec.GetHashCode() == (int)HashCode);
            TabItems.Remove(res);
        }

        # region INRC

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        # endregion
    }
}
