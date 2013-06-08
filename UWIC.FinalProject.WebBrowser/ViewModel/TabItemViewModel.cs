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

namespace UWIC.FinalProject.WebBrowser.ViewModel
{
    public class TabItemViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TabItem> _tabitems = new ObservableCollection<TabItem>();
        private int _selectedIndex = -1;

        public TabItemViewModel()
        {
            TabItem myItem = new TabItem();
            myItem.Header = getNewTabItemHeader();
            myItem.Content = new Controller.BrowserContainer();
            TabItems.Add(myItem);

            this.ClickCommand = new RelayCommand(Execute);
        }

        private TabItemHeader getNewTabItemHeader()
        {
            return new TabItemHeader(getBlankPageImage(), "New Tab");
        }

        private BitmapImage getBlankPageImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("/Images/icon-page.png", UriKind.RelativeOrAbsolute);
            image.EndInit();

            return image;
        }

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

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public ICommand ClickCommand { get; set; }

        public void Execute()
        {
            TabItem myItem = new TabItem();
            myItem.Header = getNewTabItemHeader();
            myItem.Content = new Controller.BrowserContainer();
            TabItems.Add(myItem);
            SelectedIndex = TabItems.Count - 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
