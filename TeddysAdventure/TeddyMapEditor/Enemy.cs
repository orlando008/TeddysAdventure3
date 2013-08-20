using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace TeddyMapEditor
{
    public class Enemy
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private float _velocityX;

        public float VelocityX
        {
            get { return _velocityX; }
            set 
            { 
                _velocityX = value;
                EventHandler handler = SomethingChanged;
                if (handler != null)
                {
                    handler(this, null);
                }
            }
        }

        private float _velocityY;

        public float VelocityY
        {
            get { return _velocityY; }
            set 
            { 
                _velocityY = value;
                EventHandler handler = SomethingChanged;
                if (handler != null)
                {
                    handler(this, null);
                }
            }
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

        private Rectangle _parent;

        public Rectangle Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        public event EventHandler SomethingChanged;

        public Enemy(Rectangle parent, string name, Point location, float velocityX, float velocityY)
        {
            _name = name;
            _location = location;
            _velocityX = velocityX;
            _velocityY = velocityY;
            _parent = parent;
        }

        public string GetXMLString()
        {
            string s = "";
            s += "<Item>" + System.Environment.NewLine;
            s += "<Type>" + _name + "</Type>" + System.Environment.NewLine;
            s += "<Position>" + Convert.ToInt32(_location.X).ToString() + " " + Convert.ToInt32(_location.Y).ToString() + "</Position>" + System.Environment.NewLine;
            s += "<Velocity>" + _velocityX.ToString() + " " + _velocityY.ToString() + "</Velocity>" + System.Environment.NewLine;
            s += "</Item>" + System.Environment.NewLine;

            return s;
        }
    }
}
