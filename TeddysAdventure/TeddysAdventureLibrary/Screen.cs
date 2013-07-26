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
    public class Screen : DrawableGameComponent
    {

        private string filename;
        private Texture2D sprite;
        private Texture2D _deathSprite;
        private Vector2 _position;
        private List<Rectangle> _surfaces;

        public static SpriteBatch spriteBatch;

        /// <summary>
        /// Sprite
        /// </summary>
        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public Texture2D DeathSprite
        {
            get { return _deathSprite; }
            set { _deathSprite = value; }
        }

        public List<Rectangle> Surfaces
        {
            get { return _surfaces; }
            set { _surfaces = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }


        public Screen(Game game, Vector2 position, Texture2D mystylesheet)
            : base(game)
        {
            Position = position;
            Sprite = mystylesheet;
            Surfaces = new List<Rectangle>();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public void MoveX(int speed)
        {
            Position = new Vector2(Position.X + speed, Position.Y);

            for (int i = Surfaces.Count - 1; i >= 0 ; i--)
            {
                Surfaces[i] = new Rectangle(Surfaces[i].X + speed, Surfaces[i].Y, Surfaces[i].Width, Surfaces[i].Height); 
            }

        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            Rectangle r = new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height);

            spriteBatch.Draw(Sprite, r, Color.White);


            if (((Teddy)Game.Components[1]).Dead)
            {
                r = new Rectangle(0, 0, DeathSprite.Width, DeathSprite.Height);
                spriteBatch.Draw(DeathSprite, r, Color.White);
            }


            spriteBatch.End();
        }
    }
}
