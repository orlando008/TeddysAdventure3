using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using Microsoft.Win32;
using System.Xml;
using System.ComponentModel;

namespace TeddyMapEditor
{
    class EditorViewModel:System.ComponentModel.INotifyPropertyChanged
    {

        private Color _backgroundColor = Colors.OldLace;



        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; OnPropertyChanges("BackgroundColor"); }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanges(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {

                    handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }



    }


}
