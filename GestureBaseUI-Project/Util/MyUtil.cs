using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;

namespace GestureBaseUI_Project
{
    public  class MyUtil
    {
        /// <summary>
        /// Load resource dictionary
        /// </summary>
        /// <returns></returns>
        public  static ResourceDictionary GetStringDict()
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
