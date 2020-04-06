using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GestureBaseUI_Project.Models
{
   public   class ProcessLink
    {
        public string Title { get; set; }

        private Bitmap image;
        public BitmapSource ImageData
        {
          
            get { return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(this.image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); ; }
        }
        public Bitmap Image
        {
            
            set
            {
                image = value;
            }
        }

        public IntPtr Windows { get; set; }
    }
}
