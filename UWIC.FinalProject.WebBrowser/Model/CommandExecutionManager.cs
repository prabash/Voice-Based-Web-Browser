using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.WebBrowser.ViewModel;

namespace UWIC.FinalProject.WebBrowser.Model
{
    public class CommandExecutionManager
    {
        private static BrowserContainerViewModel _bcViewModel;
        private static TabItemViewModel _tabItemViewModel;

        public CommandExecutionManager(BrowserContainerViewModel bcViewModel)
        {
            _bcViewModel = bcViewModel;
        }

        public CommandExecutionManager(){}

    public static void SetTabItemViewModel(TabItemViewModel tabItemViewModel)
        {
            _tabItemViewModel = tabItemViewModel;
        }

        public void ExecuteCommand(Dictionary<CommandType, object> identifiedCommand)
        {
            try
            {
                var command = identifiedCommand.First();
                switch (command.Key)
                {
                    case CommandType.go:
                        {
                            ExecuteGoCommand(command.Value.ToString());
                            break;
                        }
                    case CommandType.back:
                        {
                            ExecuteBackCommand();
                            break;
                        }
                    case CommandType.forth:
                        {
                            ExecuteForwardCommand();
                            break;
                        }
                    case CommandType.refresh:
                        {
                            ExecuteRefershCommand();
                            break;
                        }
                    case CommandType.stop:
                        {
                            ExecuteStopCommand();
                            break;
                        }
                    case CommandType.opennewtab:
                        {
                            _tabItemViewModel.AddTabItem();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private static void ExecuteGoCommand(string identifiedWebSite)
        {
            if (!identifiedWebSite.Contains(".com"))
                identifiedWebSite += ".com";
            var websiteName = "http://www." + identifiedWebSite;
            Uri url;
            if (Uri.TryCreate(websiteName, UriKind.RelativeOrAbsolute, out url))
            {
                //_bcViewModel.NavigateToURL(url);
            }
        }

        private static void ExecuteBackCommand()
        {
            _bcViewModel.MoveBackward();
        }

        private static void ExecuteForwardCommand()
        {
            _bcViewModel.MoveForward();
        }

        private static void ExecuteRefershCommand()
        {
            _bcViewModel.RefreshBrowserWindow();
        }

        private static void ExecuteStopCommand()
        {
            _bcViewModel.StopBrowser();
        }
    }
}
