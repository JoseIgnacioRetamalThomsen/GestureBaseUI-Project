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
        /// Queue for output images, images are just represent as an array of floats.
        /// </summary>
        private BlockingCollection<float[,]> images;

        /// <summary>
        /// Queue for output hand position
        /// </summary>
        private BlockingCollection<BodyData> bodyData;


        private const int TOTAL_PAGES = 13;

        private string[] TitlesKeys = new string[TOTAL_PAGES]
        {
            "title1","title2","title3","title4","title5","title6","title7","title8","title9","title10","title11","title12","title13"
        };
        private string[] Titles = new string[TOTAL_PAGES];

        private string[] BodyKeys = new string[TOTAL_PAGES]
        {
            "body1","body2","body3","body4","body5","body6","body7","body8","body9","body10","body11","body12","body13"
        };

        private string[] BodyTexts = new string[TOTAL_PAGES];


        private string _infoAll;

        public string InfoAll
        {
            get { return _infoAll; }
            set
            {
                SetValue(ref _infoAll, value);
            }
        }
        private int[] imagesMap = new int[TOTAL_PAGES] { 5, 0 ,1,2,3,4,6,7,8,9,10,11,11};



        private int _pageNumber = 0;

        public int PageNumber
        {
            private set { }
            get
            {
                return _pageNumber;
            }
        }

        private string _helpText;
        public string BodyText
        {
            get
            {
                return _helpText;
            }
            set
            {
                SetValue(ref _helpText, value);
            }
        }

        private string _title = "Title";
        public string Title
        {
            get { return _title; }
            set
            {
                SetValue(ref _title, value);
            }

        }

        private Color _circleColor = Colors.Yellow;

        public SolidColorBrush CircleColor
        {
            get { return new SolidColorBrush(_circleColor); }

            set
            {
                SetValue(ref _circleColor, value.Color);
            }
        }

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

        private int _waitTime = 1000;

        public int WaitTime
        {
            get { return _waitTime; }
            set
            {
                SetValue(ref _waitTime, value);
            }
        }

        private void SetGestureImage(int value)
        {
            Uri uri = new Uri(_imagesPaths[imagesMap[value]]);
            BitmapImage temp = new BitmapImage(uri);
            //need to freze image because is call from another thread.
            temp.Freeze();
            GestureTypeImage = temp;
        }

        public ICommand NextPage
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    //next page
                    GoNextPage();


                });
            }
        }

        int count = 0;
        public void GestureDone()
        {
            Debug.WriteLine("Callled");
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
        public void GoNextPage()
        {
            _pageNumber = ++_pageNumber % TOTAL_PAGES;
            ChangePage(_pageNumber);
        }

        public ICommand PreviousPage
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    //prvius page
                    _pageNumber = --_pageNumber % TOTAL_PAGES;
                    ChangePage(_pageNumber);

                });
            }
        }

        private List<string> _imagesPaths = new List<string>();
        private readonly TutorialActionManager am;
        public TutorialViewModel()
        {

            //load strings
            SetStrings(GetStringDict());

            //get images pahts
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"Images/HandGestureIcons"));
            FileInfo[] imagesPath = di.GetFiles("*.png");

            foreach (var i in imagesPath)
            {
                _imagesPaths.Add(i.FullName);
            }
            _imagesPaths.Sort();



            ChangePage(0);


            this.images = new BlockingCollection<float[,]>();
            this.bodyData = new BlockingCollection<BodyData>();


            MyRect screen = new MyRect(
            0,
            0,
            (float)System.Windows.SystemParameters.PrimaryScreenWidth,
            (float)System.Windows.SystemParameters.PrimaryScreenHeight);



            MyRect moveArea = new MyRect(250, -50, 450, 100);
            am = new TutorialActionManager(this);
            //start and create predictor
            Task.Factory.StartNew(() => { MainCamera predictor = new MainCamera(this.images, bodyData); });


            new Thread(() =>
            {
                while (true)
                {

                    // Debug.WriteLine("Count " + images.Count);
                    float[,] im = images.Take();


                    if (!isImage) continue;
                    am.AddImage(im);

                }
            }).Start();


        }

        private bool isImage = true;

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

        private void ChangePage(int v)
        {
            BodyText = BodyTexts[v];
            Title = Titles[v];
            SetGestureImage(v);
        }



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
    }
}
