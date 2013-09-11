using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TeddysAdventureLibrary
{
    public class GeometryMethods
    {

        public class RectangleF
        {
            private float _width;
            private float _height;
            private float _x;
            private float _y;

            public RectangleF(float x, float y, float width, float height) {
                _width = width;
                _height = height;
                _x = x;
                _y = y;

            }
            public RectangleF(Microsoft.Xna.Framework.Rectangle r)
            {
                _width = (float)r.Width;
                _height = (float)r.Height;
                _x = (float)r.X;
                _y = (float)r.Y;
            }

            public float Top { get { return _y ; } }
            public float Bottom { get { return _y + _height; } }
            public float X { get { return _x; } }
            public float Y { get { return _y; } }
            public float Width { get { return _width; } }
            public float Height { get { return _height; } }
            public float Left { get { return _x; } }
            public float Right { get { return _x + Width; } }


            public bool Intersects(RectangleF r)
            {
                if (this.Right < r.X || r.Right < _x || this.Bottom < r.Y || r.Bottom < _y)
                    return false;
                else
                    return true;
            }

            public bool Intersects(Rectangle r)
            {
                if (this.Right <= r.X || r.Right <= _x || this.Bottom <= r.Y || r.Bottom <= _y)
                    return false;
                else
                    return true;
            }

        }

        

        static bool PointIsInsideRectangleF( float x, float y, RectangleF r )  {

            bool inside = false;

            if (( x > r.X ) && ( x < r.X + r.Width ) && ( y > r.Y ) && (y < r.Y + r.Height ) ){
                inside = true;
            }
            
            return inside;
        }

        static bool PointIsInsideRectangle( float x, float y, Rectangle r )  {

            bool inside = false;

            if (( x > r.X ) && ( x < r.X + r.Width ) && ( y > r.Y ) && (y < r.Y + r.Height ) ){
                inside = true;
            }
            
            return inside;
        }


    }
}
