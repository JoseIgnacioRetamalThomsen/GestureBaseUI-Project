
using GestureBaseUI_Project.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace GestureBaseUI_Project.ViewModel
{
    /// <summary>
    /// Home page view model
    /// </summary>
    public class HomeViewModel :BaseViewModel
    {

        private bool _panelLoading;
        private string _panelMainMessage = "Main Loading Message";
        private string _panelSubMessage = "Sub Loading Message";

    
        public bool PanelLoading
        {
            get
            {
                return _panelLoading;
            }
            set
            {
                SetValue(ref _panelLoading, value);
            }
        }

        public string PanelMainMessage
        {
            get
            {
                return _panelMainMessage;
            }
            set
            {
                SetValue(ref _panelMainMessage, value);
            }
        }

        public string PanelSubMessage
        {
            get
            {
                return _panelSubMessage;
            }
            set
            {
                SetValue(ref _panelSubMessage, value);
            }
        }

        public ICommand PanelCloseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {                   
                    Debug.WriteLine("working");
                    PanelLoading = false;
                });
            }
        }

        public ICommand ShowPanelCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    PanelLoading = true;
                });
            }
        }

        public ICommand HidePanelCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    PanelLoading = false;
                });
            }
        }

        public ICommand ChangeSubMessageCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    PanelSubMessage = string.Format("Message: {0}", DateTime.Now);
                });
            }
        }

    }
}
