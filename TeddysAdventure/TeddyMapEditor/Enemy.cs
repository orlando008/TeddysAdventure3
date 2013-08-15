using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

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
            set { _velocityX = value; }
        }

        private float _velocityY;

        public float VelocityY
        {
            get { return _velocityY; }
            set { _velocityY = value; }
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

        public Enemy(string name, Point location, float velocityX, float velocityY)
        {
            _name = name;
            _location = location;
            _velocityX = velocityX;
            _velocityY = velocityY;
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
