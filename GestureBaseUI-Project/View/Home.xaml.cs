using System;
using System.Collections.Generic;
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
using System.Speech.Recognition;
using GestureBaseUI_Project.ViewModel;

namespace GestureBaseUI_Project
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        /// <summary>
        /// The View Model
        /// </summary>
        private readonly HomeViewModel _viewModel;
        public Home()
        {
            InitializeComponent();

            //Create and sets the view model.
            _viewModel = new HomeViewModel();
            DataContext = _viewModel;
            
        }

        #region Navigation
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PictureRecorder());
        }

        private void Recorder_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Recorder());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Prediction());
        }
        #endregion
    }
}
