using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class GameObject : DrawableGameComponent
    {
        private Texture2D _styleSheet;
        private Game _game;
        private Vector2 _position;
        private Rectangle _collisionBox;
        private Rectangle _boxToDraw;
        private Vector2 _frameSize;
        private bool _destroyed;

        public Texture2D StyleSheet
        {
            get { return _styleSheet; }
            set { _styleSheet = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }

        public Vector2 FrameSize
        {
            get { return _frameSize; }
            set { _frameSize = value; }
        }

        public Rectangle BoxToDraw
        {
            get { return _boxToDraw; }
            set { _boxToDraw = value; }
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, BoxToDraw.Width, BoxToDraw.Height);
            }
        }

        public bool Destroyed
        {
            get { return _destroyed; }
            set
            {
                _destroyed = value;
                if (value == true)
                {
                    Position = new Vector2(-100, -100);
                }
            }
        }

        public GameObject(Game game, Vector2 position)
            : base(game)
        {
            Position = position;
            
            Destroyed = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch sp)
        {
            if (!_destroyed)
            {
                sp.Draw(StyleSheet, Position, BoxToDraw, Color.White);
            }
        }

    }
}
