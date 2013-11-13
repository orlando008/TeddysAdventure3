using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary
{
    public class Powerup
    {
        protected Game _game;

        public Powerup(Game game)
        {
            _game = game;
        }

        public virtual bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {
            return true;
        }

        
        public virtual bool BeforeDraw(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {
            return true;
        }

        public virtual void AfterDraw(GameTime gameTime, SpriteBatch batch, Teddy teddy, SpriteEffects seff)
        {
            return;
        }

        public virtual GeometryMethods.RectangleF GetExpandedPowerupRectangle(Teddy teddy)
        {
            return teddy.JustTeddyRectangle;
        }

        public virtual bool HandleEnemyInteraction(Enemy e, Screen currentScreen, GeometryMethods.RectangleF enemyHitBox) {
            return true;
        }

        public virtual bool HandleLandingOnEnemy(Enemy e, KeyboardState keyState) {
            return true;
        }
    }
}
