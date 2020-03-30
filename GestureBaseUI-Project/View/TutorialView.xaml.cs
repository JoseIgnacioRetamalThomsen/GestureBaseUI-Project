using GestureBaseUI_Project.ViewModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;


namespace GestureBaseUI_Project.View
{
    /// <summary>
    /// Interaction logic for TutorialView.xaml
    /// </summary>
    public partial class TutorialView : Page
    {
        private readonly TutorialViewModel _viewModel;
        public TutorialView()
        {
            InitializeComponent();

            _viewModel = new TutorialViewModel();
            DataContext = _viewModel;
            this.Unloaded += _viewModel.TutorialView_Unloaded;
            
        }

        private void TutorialView_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("unloaded xxxxxxxxxxxxxxxxxxxxxxxx");
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Home());
        }
    }
}
