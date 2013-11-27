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

            public virtual float Top { get { return _y ; } }
            public virtual  float Bottom { get { return _y + _height; } }
            public float X { get { return _x; } }
            public float Y { get { return _y; } }
            public virtual  float Width { get { return _width; } }
            public virtual  float Height { get { return _height; } }
            public virtual  float Left { get { return _x; } }
            public virtual  float Right { get { return _x + Width; } }


            public virtual bool Intersects(RectangleF r)
            {
                if (this.Right < r.X || r.Right < _x || this.Bottom < r.Y || r.Bottom < _y)
                    return false;
                else
                    return true;
            }

            public virtual bool Intersects(Rectangle r)
            {
                if (this.Right <= r.X || r.Right <= _x || this.Bottom <= r.Y || r.Bottom <= _y)
                    return false;
                else
                    return true;
            }

            public Rectangle AsRect()
            {
                return new Rectangle((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);

            }

        }

        public class MultiRectangleF : RectangleF
        {

            private RectangleF[] _subRects;

            public MultiRectangleF(float width, float height, RectangleF[] subRects)
                : base(0,0, width, height)
            {
                _subRects = subRects;
            }

            public override bool Intersects(Rectangle r)
            {
                foreach (RectangleF subR in _subRects)
                {
                    if (subR.Intersects(r))
                    {
                        return true;
                    }

                }
                
                return false;
            }

            public override bool Intersects(RectangleF r)
            {
                foreach (RectangleF subR in _subRects)
                {
                    if (subR.Intersects(r))
                    {
                        return true;
                    }

                }
                
                return false;
            }

            public override float Bottom
            {
                get
                {
                    return _subRects.Max(r => r.Bottom);
                }
            }

            public override float Top
            {
                get
                {
                    return _subRects.Min(r => r.Top);
                }
            }

            public override float Left
            {
                get
                {
                    return _subRects.Min(r => r.Left);
                }
            }

            public override float Right
            {
                get
                {
                    return _subRects.Max(r => r.Right);
                }
            }
            public override float Height
            {
                get
                {
                    return this.Bottom - this.Top;
                }
            }

            public override float Width
            {
                get
                {
                    return this.Right - this.Left;
                }
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
