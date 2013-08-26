using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace TeddysAdventureLibrary
{
    public class Surface
    {
        public Rectangle Rect { get; set; }
        public string Sprite { get; set; }

        //Wrap rect properties
        public int X { get { return this.Rect.X; } }
        public int Y { get { return this.Rect.Y; } }
        public int Width { get { return this.Rect.Width; } }
        public int Height { get { return this.Rect.Height; } }
        public int Left { get { return this.Rect.Left; } }
        public int Right { get { return this.Rect.Right; } }
        public int Top { get { return this.Rect.Top; } }
        public int Bottom { get { return this.Rect.Bottom; } }


        public Surface(SurfaceHelper sh)
        {
            this.Rect = sh.Rect;
            this.Sprite = sh.Sprite;
        }

        public void MoveByX(int x)
        {
            this.Rect = new Rectangle( this.Rect.X + x, this.Rect.Y, this.Rect.Width, this.Rect.Height);
        }

    }
}
