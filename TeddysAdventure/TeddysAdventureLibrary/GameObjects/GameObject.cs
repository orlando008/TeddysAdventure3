﻿using System;
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

#if COLLISIONS
        protected Texture2D _redFill;
#endif

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

        public GeometryMethods.RectangleF CollisionRectangle
        {
            get
            {
                return new GeometryMethods.RectangleF((int)Position.X, (int)Position.Y, BoxToDraw.Width, BoxToDraw.Height);
            }
        }

        public Rectangle CollisionRectangleRegular
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, BoxToDraw.Width, BoxToDraw.Height);
            }
        }

        public Rectangle DestinationBoxToDraw
        {
            get
            {
                return new Rectangle((int)_position.X + _boxToDraw.Width / 2, (int)_position.Y + _boxToDraw.Height / 2, (int)_boxToDraw.Width, (int)_boxToDraw.Height);
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

#if COLLISIONS
            _redFill = new Texture2D(game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _redFill.SetData<Color>(new Color[] { Color.Red });
#endif
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch sp)
        {
            if (!_destroyed)
            {
                sp.Draw(StyleSheet, Position, BoxToDraw, Color.White);
            }
        }

    }
}
