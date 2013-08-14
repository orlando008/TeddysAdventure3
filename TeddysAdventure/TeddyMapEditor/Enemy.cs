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

        public Enemy(string name, Point location, float velocityX, float velocityY)
        {
            _name = name;
            _location = location;
            _velocityX = velocityX;
            _velocityY = velocityY;
        }
    }
}
