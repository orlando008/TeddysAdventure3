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

        public Texture2D StyleSheet
        {
            get { return _styleSheet; }
            set { _styleSheet = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
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

        public Fluff(Game game, Vector2 position)
            : base(game)
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");
            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            spriteBatch.Draw(StyleSheet, Position, BoxToDraw, Color.White);
            spriteBatch.End();
        }
    }
}
