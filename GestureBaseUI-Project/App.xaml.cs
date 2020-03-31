
using GestureBaseUI_Project;
using GestureBaseUI_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GestureBaseUI_Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ResourceDictionary resdict;

        public static UserData userdata;
        public App()
        {
            resdict = MyUtil.GetStringDict();
            this.Resources.MergedDictionaries.Add(resdict);
            LoadUserData();
        }

        private void LoadUserData()
        {
            userdata = new UserData()
            {
                Speed = 10,
                ShortCut1 = "C:/Program Files (x86)/Google/Chrome/Application/chrome.exe",
                ShortCut2 = "C:/Program Files/Mozilla Firefox/firefox.exe"
            };
        }
    }
}
