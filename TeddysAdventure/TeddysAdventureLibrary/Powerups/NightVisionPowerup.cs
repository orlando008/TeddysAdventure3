using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TeddysAdventureLibrary
{
    class NightVisionPowerup:Powerup
    {

        private Texture2D _styleSheet;
        private Rectangle _boxToDraw;
        private Texture2D _overlaySprite;

        public NightVisionPowerup(Game game)
            : base(game)
        {
            _styleSheet = game.Content.Load<Texture2D>("Objects\\NightVision");
            _overlaySprite = game.Content.Load<Texture2D>("Screens\\tintOverlay");
            _boxToDraw = new Rectangle(50, 0, 50, 44);
        }

        public override void AfterDraw(GameTime gameTime, SpriteBatch batch, Teddy teddy, SpriteEffects seff)
        {

            if (teddy.Facing == Teddy.Direction.Left || teddy.Facing == Teddy.Direction.Right)
            {
                batch.Draw(_styleSheet, teddy.Position, new Rectangle(0, 0, 50, 44), Color.White, 0, Vector2.Zero, 1, seff, 0);
            }
            else
            {
                batch.Draw(_styleSheet, teddy.Position, new Rectangle(50, 0, 50, 44), Color.White, 0, Vector2.Zero, 1, seff, 0);
            }
        }

        public override void ScreenDraw(GameTime gameTime, SpriteBatch batch, Teddy teddy, SpriteEffects seff)
        {
            Color dc;
            dc = new Color(Color.DarkGreen.R, Color.DarkGreen.G, Color.DarkGreen.B, 10);

            batch.Draw(_overlaySprite, new Rectangle(0, 0, _overlaySprite.Width, _overlaySprite.Height), dc);

            //base.ScreenDraw(gameTime, batch, teddy, seff);
        }


    }
}
