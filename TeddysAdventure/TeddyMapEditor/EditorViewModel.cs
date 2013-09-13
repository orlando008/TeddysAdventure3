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

    interface IMovableGameItem
    {
         Point Location { get; set; }
         Rectangle Parent { get; set; }
    }


    class EditorViewModel:System.ComponentModel.INotifyPropertyChanged
    {

        private Color _backgroundColor = Colors.OldLace;
        private int _levelWidth = 1250;
        private int _levelHeight = 750;

        private IMovableGameItem _currentSelection;

        private MainWindow _view;


        public EditorViewModel(MainWindow view)
        {
            _view = view;
        }


        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; OnPropertyChanges("BackgroundColor"); }
        }

        public int LevelWidth{
         get { return _levelWidth;}
            set { _levelWidth = value; OnPropertyChanges("LevelWidth");_view.DrawGrid();  }
        }

        public int LevelHeight
        {
            get { return _levelHeight; }
            set { _levelHeight = value; OnPropertyChanges("LevelHeight"); _view.DrawGrid(); }
        }

        public IMovableGameItem CurrentSelection
        {
            get { return _currentSelection; }
            set
            {
                _currentSelection = value;
                OnPropertyChanges("CurrentSelection");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanges(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {

                    handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }



    }


}
