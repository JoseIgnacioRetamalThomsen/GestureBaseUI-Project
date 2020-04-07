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
        /// <summary>
        /// The view model
        /// </summary>
        private readonly TutorialViewModel _viewModel;
        public TutorialView()
        {
            InitializeComponent();

            _viewModel = new TutorialViewModel();
            DataContext = _viewModel;
            this.Resources.MergedDictionaries.Add(App.resdict);
            this.Unloaded += _viewModel.TutorialView_Unloaded;
            
                    }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Home());
        }
    }
}
