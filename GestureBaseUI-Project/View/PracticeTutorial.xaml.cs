using GestureBaseUI_Project.ViewModel;
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

namespace GestureBaseUI_Project.View
{
    /// <summary>
    /// Interaction logic for PracticeTutorial.xaml
    /// </summary>
    public partial class PracticeTutorial : Page
    {
        private readonly PracticeTutorialViewModel _viewModel;
        public PracticeTutorial()
        {
            InitializeComponent();
            _viewModel = new PracticeTutorialViewModel();
            this.DataContext = _viewModel;
            this.Resources.MergedDictionaries.Add(App.resdict);
        }
    }
}
