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

        public Point Location
        {
            get { return _location; }
            set 
            {
                EventHandler handler = SomethingChanged;
                _location = value;
                if (handler != null)
                {
                    handler(this, null);
                }
            }
        }

        public event EventHandler SomethingChanged;

        public GameObject(string name, Point location)
        {
            Name = name;
            Location = location;
        }
    }
}
