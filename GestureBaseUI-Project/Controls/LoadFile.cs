using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GestureBaseUI_Project.Controls
{
   public  class LoadFile
    {
        public LoadFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".exe";
            dlg.Filter = "*.exe|*.bat";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                Debug.WriteLine("file name: " + filename);
                Process.Start(filename);
            }
            else
            {
                Debug.WriteLine("noooooooooooooooooo");
            }
        }
    }
}
