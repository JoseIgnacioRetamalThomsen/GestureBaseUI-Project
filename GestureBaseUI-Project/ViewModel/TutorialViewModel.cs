using GalaSoft.MvvmLight.Messaging;
using GestureBaseUI_Project.Camera;
using GestureBaseUI_Project.Helper;
using GestureBaseUI_Project.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GestureBaseUI_Project.ViewModel
{
    public class TutorialViewModel : BaseViewModel
    {

        /// <summary>
        /// Manage the action of the tutorial, read the inputs.
        /// </summary>
        private readonly TutorialActionManager am;

        /// <summary>
        /// Runs the camera for get imagess.
        /// </summary>
        private MainCamera predictor;

        /// <summary>
        /// Queue for output images, images are just represent as an array of floats.
        /// </summary>
        private BlockingCollection<float[,]> images;

        /// <summary>
        /// Queue for output hand position
        /// </summary>
        private BlockingCollection<BodyData> bodyData;

        /// <summary>
        /// Total amount of pages in tutorial
        /// </summary>
        private const int TOTAL_PAGES = 13;

        /// <summary>
        /// Keys to title strings.
        /// </summary>
        private string[] TitlesKeys = new string[TOTAL_PAGES]
        {
            "title1","title2","title3","title4","title5","title6","title7","title8","title9","title10","title11","title12","title13"
        };

        /// <summary>
        /// Title strings, this are loaded from resource using the keys.
        /// </summary>
        private string[] Titles = new string[TOTAL_PAGES];

        /// <summary>
        /// Keys to body text.
        /// </summary>
        private string[] BodyKeys = new string[TOTAL_PAGES]
        {
            "body1","body2","body3","body4","body5","body6","body7","body8","body9","body10","body11","body12","body13"
        };

        /// <summary>
        /// Body texts, , this are loaded from resource using the keys.
        /// </summary>
        private string[] BodyTexts = new string[TOTAL_PAGES];

        /// <summary>
        /// Map image number to the one using in the tutorial page.
        /// </summary>
        private int[] imagesMap = new int[TOTAL_PAGES] { 5, 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12 };

        /// <summary>
        /// Path to all images.
        /// </summary>
        private List<string> _imagesPaths = new List<string>();

        /// <summary>
        /// Actions count
        /// </summary>
        private int count = 0;

        //  private bool isImage = true;

        /// <summary>
        /// Info text that shows in all pages
        /// </summary>
        private string _infoAll;
        public string InfoAll
        {
            get { return _infoAll; }
            set
            {
                SetValue(ref _infoAll, value);
            }
        }

        /// <summary>
        /// Actual page number.
        /// </summary>
        private int _pageNumber = 0;

        public int PageNumber
        {
            private set { }
            get
            {
                return _pageNumber;
            }
        }

        /// <summary>
        /// Main body text.
        /// </summary>
        private string _bodyText;
        public string BodyText
        {
            get
            {
                return _bodyText;
            }
            set
            {
                SetValue(ref _bodyText, value);
            }
        }

        /// <summary>
        /// Page title.
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                SetValue(ref _title, value);
            }

        }

        /// <summary>
        /// Color of the circle.
        /// </summary>
        private Color _circleColor = Colors.Yellow;

        public SolidColorBrush CircleColor
        {
            get { return new SolidColorBrush(_circleColor); }

            set
            {
                SetValue(ref _circleColor, value.Color);
            }
        }

        /// <summary>
        /// Gesture icon image
        /// </summary>
        private ImageSource _gestureTypeImage;
        public ImageSource GestureTypeImage
        {
            get
            {
                return this._gestureTypeImage;
            }
            set
            {
                SetValue(ref _gestureTypeImage, value);
            }
        }

        /// <summary>
        /// time to wait beetwen actions.
        /// </summary>
        private int _waitTime = 1000;

        public int WaitTime
        {
            get { return _waitTime; }
            set
            {
                SetValue(ref _waitTime, value);
            }
        }

        /// <summary>
        /// Control runnig of thread that take images from queue.
        /// </summary>
        private bool isRunning = true;

        /// <summary>
        /// Go to next page
        /// </summary>
        public ICommand NextPage
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    //next page
                    count = 0;
                    GoNextPage();


                });
            }
        }

        /// <summary>
        /// Go to prevous page
        /// </summary>
        public ICommand PreviousPage
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    //prvius page
                    count = 0;
                    GoPreviousPage();


                });
            }
        }

        /// <summary>
        /// Create the view model.
        /// </summary>
        public TutorialViewModel()
        {

            //load strings
            SetStrings(GetStringDict());

            //get images pahts
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"Images/HandGestureIcons"));
            FileInfo[] imagesPath = di.GetFiles("*.png");
            Debug.WriteLine("images total: " + imagesPath.Length);

            // set all images path
            foreach (var i in imagesPath)
            {
                _imagesPaths.Add(i.FullName);
            }

            // sort images paths
            _imagesPaths.Sort();

            // start at page 0
            ChangePage(0);

            //Start queues.
            this.images = new BlockingCollection<float[,]>();
            this.bodyData = new BlockingCollection<BodyData>();

            // Screen windwos
            MyRect screen = new MyRect(
            0,
            0,
            (float)System.Windows.SystemParameters.PrimaryScreenWidth,
            (float)System.Windows.SystemParameters.PrimaryScreenHeight);

            // hand movin area
            MyRect moveArea = new MyRect(250, -50, 450, 100);

            // create action manageer
            am = new TutorialActionManager(this);

            //start and create predictor
            Task.Factory.StartNew(() => { predictor = new MainCamera(this.images, bodyData); });

            // start thread that take images from queue.
            new Thread(() =>
            {
                while (isRunning)
                {
                    float[,] im = images.Take();

                    //if (!isImage) continue;
                    am.AddImage(im);
                }
            }).Start();
        }

        /// <summary>
        /// Call from manager when a new action is done.
        /// </summary>
        public void GestureDone()
        {
            //first
            if (count == 0)
            {
                CircleColor = new SolidColorBrush(Colors.Green);
                Thread.Sleep(WaitTime);
                CircleColor = new SolidColorBrush(Colors.Yellow);
                count++;
                while (images.Count > 0)
                {
                    images.Take();
                }
                am.MakeReady();
            }
            else if (count == 1)
            {
                CircleColor = new SolidColorBrush(Colors.Green);
                Thread.Sleep(WaitTime);
                CircleColor = new SolidColorBrush(Colors.Yellow);
                while (images.Count > 0)
                {
                    images.Take();
                }

                GoNextPage();
                am.MakeReady();
                count = 0;
            }
        }

        /// <summary>
        /// Go to next page.
        /// </summary>
        public void GoNextPage()
        {
            _pageNumber = ++_pageNumber % TOTAL_PAGES;
            ChangePage(_pageNumber);
        }

        /// <summary>
        /// Go to previous page.
        /// </summary>
        private void GoPreviousPage()
        {
            _pageNumber = --_pageNumber % TOTAL_PAGES;
            ChangePage(_pageNumber);
        }

        /// <summary>
        /// Set the gesture image in the view.
        /// </summary>
        /// <param name="value"></param>
        private void SetGestureImage(int value)
        {
            Uri uri = new Uri(_imagesPaths[imagesMap[value]]);
            BitmapImage temp = new BitmapImage(uri);
            //need to freze image because is call from another thread.
            temp.Freeze();
            GestureTypeImage = temp;
        }

        /// <summary>
        /// Load string from resources.
        /// </summary>
        /// <param name="dict"></param>
        private void SetStrings(ResourceDictionary dict)
        {
            InfoAll = dict["infoall"].ToString();
            //set title
            for (int i = 0; i < TOTAL_PAGES; i++)
            {
                Titles[i] = dict[TitlesKeys[i]].ToString();
            }

            //set bodies
            for (int i = 0; i < TOTAL_PAGES; i++)
            {
                BodyTexts[i] = dict[BodyKeys[i]].ToString();
            }
        }

        /// <summary>
        /// Change a page.
        /// </summary>
        /// <param name="v"></param>
        private void ChangePage(int v)
        {
            BodyText = BodyTexts[v];
            Title = Titles[v];
            SetGestureImage(v);
        }

        /// <summary>
        /// Load resource dictionary
        /// </summary>
        /// <returns></returns>
        private ResourceDictionary GetStringDict()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("..\\Resources\\Tutorial.xaml",
                                  UriKind.Relative);
                    break;
                case "fr-CA":
                    dict.Source = new Uri("..\\Resources\\StringResources.fr-CA.xaml",
                                       UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\Tutorial.xaml",
                                      UriKind.Relative);
                    break;
            }
            return dict;

        }
               
        /// <summary>
        /// Stop threads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TutorialView_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.isRunning = false;
            this.predictor.Close();
        }
    }
}
