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
        protected bool _preventsJump = false;

        public Powerup(Game game)
        {
            _game = game;
        }

        public virtual bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {
            return true;
        }

        public virtual void AfterUpdate(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {
            return;
        }

        public virtual bool BeforeDraw(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {
            return true;
        }

        public virtual void AfterDraw(GameTime gameTime, SpriteBatch batch, Teddy teddy, SpriteEffects seff)
        {
            return;
        }

        public virtual void ScreenDraw(GameTime gameTime, SpriteBatch batch, Teddy teddy, SpriteEffects seff)
        {
            return;
        }

        public virtual GeometryMethods.RectangleF GetExpandedPowerupRectangle(Teddy teddy)
        {
            return teddy.JustTeddyRectangle;
        }

        public virtual bool HandleEnemyInteraction(Teddy teddy, Enemy e, Screen currentScreen, GeometryMethods.RectangleF enemyHitBox) {
            return true;
        }

        //Return true if damage make you lose powerup
        public virtual bool AfterTeddyDamage(Teddy teddy)
        {
            return true;
        }

        public virtual bool HandleLandingOnEnemy(Enemy e, KeyboardState keyState) {
            return true;
        }

        public virtual bool HandleFluffGrab(Teddy teddy, Fluff f)
        {
            return true;
        }

        public virtual bool PreventsJump(Teddy teddy)
        {
            return _preventsJump;
        }
    }
}
