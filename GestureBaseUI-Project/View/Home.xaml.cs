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
using System.Speech.Recognition;
using System.Globalization;

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

        //Adapted from https://docs.microsoft.com/en-us/dotnet/api/system.speech.recognition.speechrecognitionengine.recognize?view=netframework-4.8
        public void Button_Voice_Command_On(object sender, RoutedEventArgs e)
        {
            string text;
    
            // Create an in-process speech recognizer for the en-US locale.
            using (SpeechRecognitionEngine recognizer =
            new SpeechRecognitionEngine())
                {
                    // Configure input to the speech recognizer.
                    recognizer.SetInputToDefaultAudioDevice();

                    // Modify the time value. 
                    recognizer.BabbleTimeout = TimeSpan.FromSeconds(0);
                    recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
                    recognizer.EndSilenceTimeout = TimeSpan.FromSeconds(0.5);

                    // Create and load a dictation grammar.  
                    recognizer.LoadGrammar(new DictationGrammar());

                    // Start synchronous speech recognition.  
                    RecognitionResult result = recognizer.Recognize();

                    if (result != null)
                    {
                    text = result.Text.ToLower();
                        MessageBox.Show(text);
                        if (text.Contains("record") == true)
                        {
                        this.NavigationService.Navigate(new Recorder());
                    }
                    else if(text.Contains("record") == true)
                        {
                         this.NavigationService.Navigate(new Prediction());
                        }
                    else if (text.Contains("view") == true)
                        {
                        this.NavigationService.Navigate(new PictureRecorder());
                        }
                    }
                    else
                    {
                       //If result does not match
                        MessageBox.Show("No Spoken word");
                    }
                }

    
        }
        #endregion
    }
}
