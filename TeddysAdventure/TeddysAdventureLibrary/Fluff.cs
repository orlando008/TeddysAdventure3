using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class Fluff : DrawableGameComponent
    {
        public static SpriteBatch spriteBatch;
        private Texture2D _styleSheet;
        private Game _game;
        private Vector2 _position;
        private Rectangle _collisionBox;
        private Rectangle _boxToDraw;
        private Vector2 _frameSize;
        private bool _destroyed;
        private int _floatCount;
        private int _centerLine;
        private int _lengthOfPose = 20;

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

        public Fluff(Game game, Vector2 position)
            : base(game)
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");
            Position = position;
            _centerLine = (int)position.X;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            Destroyed = false;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_destroyed)
            {
                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                spriteBatch.Draw(StyleSheet, Position, BoxToDraw, Color.White);
                spriteBatch.End();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!_destroyed)
            {
                if (_floatCount < _lengthOfPose)
                {
                    Position = new Vector2(_centerLine, Position.Y);
                    _floatCount++;
                }
                else if (_floatCount < _lengthOfPose * 2)
                {
                    Position = new Vector2(_centerLine - 1, Position.Y);
                    _floatCount++;
                }
                else if (_floatCount < _lengthOfPose * 3)
                {
                    Position = new Vector2(_centerLine, Position.Y);
                    _floatCount++;
                }
                else if (_floatCount < _lengthOfPose * 4)
                {
                    Position = new Vector2(_centerLine + 1, Position.Y);
                    _floatCount++;
                }
                else
                {
                    _floatCount = 0;
                }
                
            }
        }

        public void MoveFluffByX(int x)
        {
            this.Position = new Vector2((int)Position.X + x, (int)Position.Y);
            _centerLine = _centerLine + x;
        }

        public void MoveFluffByY(int y)
        {
            this.Position = new Vector2((int)Position.X, (int)Position.Y + y);
        }
    }
}
