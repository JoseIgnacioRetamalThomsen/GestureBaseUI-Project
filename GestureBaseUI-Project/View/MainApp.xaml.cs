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
            _viewModel = new MainAppViewModel();
            DataContext = _viewModel;

            LinksView.ItemsSource = _viewModel.Links;
        }
     
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("here" + ((ProcessLink)((ListViewItem)sender).DataContext).Windows) ;
            WindowController.Instance.OpenWindow(((ProcessLink)((ListViewItem)sender).DataContext).Windows);

        }
    }
}
