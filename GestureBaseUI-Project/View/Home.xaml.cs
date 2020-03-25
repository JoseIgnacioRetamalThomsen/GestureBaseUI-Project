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
using System.Windows;



namespace GestureBaseUI_Project
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();

           // box.ItemsSource = Enum.GetValues(typeof(HandGestures));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // View Expense Report
            PictureRecorder pr = new PictureRecorder();
            this.NavigationService.Navigate(pr);
        }

        private void Recorder_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Recorder());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Prediction());
        }
    }
}
