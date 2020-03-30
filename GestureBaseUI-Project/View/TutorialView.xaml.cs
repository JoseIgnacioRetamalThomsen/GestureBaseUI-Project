using GestureBaseUI_Project.ViewModel;
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
        }
    }
}
