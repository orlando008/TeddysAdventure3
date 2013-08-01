using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TeddysAdventureLibrary
{
    public class StartMenu : DrawableGameComponent
    {
        private Texture2D sprite;
        private Texture2D _cursorSprite;
        private Vector2 _position;

        private Boolean _started = false;

        public static SpriteBatch spriteBatch;

        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public Texture2D CursorSprite
        {
            get { return _cursorSprite; }
            set { _cursorSprite = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Boolean Started
        {
            get { return _started; }
            set { _started = value; }
        }

        public StartMenu(Game game, Vector2 position, Texture2D myStyleSheet)
            : base(game)
        {
            Position = position;
            Sprite = myStyleSheet;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            Rectangle r = new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height);

            spriteBatch.Draw(Sprite, r, Color.White);

            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Enter))
            {
                _started = true;
            }
        }
    }
}
