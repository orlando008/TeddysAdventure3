using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TeddyMapEditor
{
    public class GameObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Point _location;

        public GameObject(string name, Point location)
        {
            _name = name;
            _location = location;
        }
    }
}
