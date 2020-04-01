using GalaSoft.MvvmLight.Messaging;
using GestureBaseUI_Project.Models;
using GestureBaseUI_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestureBaseUI_Project.View
{
    /// <summary>
    /// Interaction logic for MainApp.xaml
    /// </summary>
    public partial class MainApp : Page
    {
        /// <summary>
        /// The View Model
        /// </summary>
        private readonly MainAppViewModel _viewModel;

        public MainApp()
        {
            InitializeComponent();

            //Create and sets the view model.
            _viewModel = new MainAppViewModel(this);
            DataContext = _viewModel;

            Resources.MergedDictionaries.Add(App.resdict);

            LinksView.ItemsSource = _viewModel.Links;

            Unloaded += _viewModel.MainApp_Unloaded;

            
                Messenger.Default.Register<NavigateRequest>(
                  this,
                  NavigateHome);
            
        }

        private void NavigateHome(NavigateRequest obj)
        {
            try
            {


                Dispatcher.Invoke(new Action(() => { this.NavigationService.Navigate(new Home()); }));
            }
            catch { }
            
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
     
            WindowController.Instance.OpenWindow(((ProcessLink)((ListViewItem)sender).DataContext).Windows);

        }

        
        private void BackButton(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Home());
        }
    }
}
