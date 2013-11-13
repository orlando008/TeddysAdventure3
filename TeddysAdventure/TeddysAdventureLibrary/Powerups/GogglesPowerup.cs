using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TeddysAdventureLibrary
{
    class GogglesPowerup:Powerup
    {

        private Texture2D _styleSheet;
        private Rectangle _boxToDraw;

        public GogglesPowerup(Game game)
            : base(game)
        {
            _styleSheet = game.Content.Load<Texture2D>("Objects\\Goggles");

            _boxToDraw = new Rectangle(50, 0, 50, 28);
        }

        public override bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {

                if (keyState.IsKeyDown(Keys.Down) && teddy.RidingSurface != null && teddy.RidingSurface.SurfaceOwner() != null)
                {
                    teddy.RidingSurface.SurfaceOwner().PlayerIsSteeringEnemyDown(2, 1);
                }

                if (keyState.IsKeyDown(Keys.Up) && teddy.RidingSurface != null && teddy.RidingSurface.SurfaceOwner() != null)
                {
                    teddy.RidingSurface.SurfaceOwner().PlayerIsSteeringEnemyDown(2, -1);
                }


            return true;
        }

        public override void AfterDraw(GameTime gameTime, SpriteBatch batch, Teddy teddy, SpriteEffects seff)
        {

                if (teddy.Facing == Teddy.Direction.Left || teddy.Facing == Teddy.Direction.Right)
                {
                    batch.Draw(_styleSheet, teddy.Position, new Rectangle(100, 0, 50, 53), Color.White, 0, Vector2.Zero, 1, seff, 0);
                }
                else
                {
                    batch.Draw(_styleSheet, teddy.Position, new Rectangle(0, 0, 50, 53), Color.White, 0, Vector2.Zero, 1, seff, 0);
                }

        }


    }
}
