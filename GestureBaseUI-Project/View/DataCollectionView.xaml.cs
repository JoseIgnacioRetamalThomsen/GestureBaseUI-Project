using GestureBaseUI_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DataCollectionView.xaml
    /// </summary>
    public partial class DataCollectionView : Page
    {
        /// <summary>
        /// The view model
        /// </summary>
        private readonly DataCollectionViewModel _viewModel;
        public DataCollectionView()
        {
            InitializeComponent();
            _viewModel = new DataCollectionViewModel();
            DataContext = _viewModel;
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
