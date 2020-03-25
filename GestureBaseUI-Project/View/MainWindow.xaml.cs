using System.Windows;

namespace GestureBaseUI_Project
{
    /// <summary>
    /// Main Frame for appliction navigation.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.InitializeComponent();

            // Set Home page
            _mainFrame.Navigate(new Home());

        }



    }
}
