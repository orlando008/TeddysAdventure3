using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace TeddyMapEditor
{
    public class GameObject : IMovableGameItem
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
                _location = value;
                EventHandler handler = SomethingChanged;
                if (handler != null)
                {
                    handler(this, null);
                }
            }
        }

        private Rectangle _parent;

        public Rectangle Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        public event EventHandler SomethingChanged;

        public GameObject(Rectangle parent, string name, Point location)
        {
            Name = name;
            Location = location;
            Parent = parent;
        }

        public string GetXMLString()
        {
            string s = "";
            s += "<Item>" + System.Environment.NewLine;
            s += "<Type>" + _name + "</Type>" + System.Environment.NewLine;
            s += "<Position>" + Convert.ToInt32(_location.X).ToString() + " " + Convert.ToInt32(_location.Y).ToString() + "</Position>" + System.Environment.NewLine;
            s += "</Item>" + System.Environment.NewLine;

            return s;
        }
    }
}
