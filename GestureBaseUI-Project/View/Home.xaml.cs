using System.Windows;
using System.Windows.Controls;
using GestureBaseUI_Project.View;
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
        private void Tutorial_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new TutorialView());
        }

        private void Recorder_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Recorder());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Prediction());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainApp());
        }

        #endregion

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new DataCollectionView());
        }
    }
}
