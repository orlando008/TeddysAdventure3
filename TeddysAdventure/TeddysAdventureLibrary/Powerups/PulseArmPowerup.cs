using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary
{
    class PulseArmPowerup:Powerup
    {

        protected bool _firingPulseArm = false;
        protected bool _drawFiringSprite = false;
        protected int _pulseCoolDownCounter = 0;
        protected int _pulseCoolDown = 50;
        protected Keys _pulseKey = Keys.F;
        protected int _pulseVelocity = 4;
        protected Texture2D _pulseArmSprites;

        public PulseArmPowerup(Game game):base(game){
            _firingPulseArm = false;
            _pulseCoolDownCounter = 0;
            _pulseArmSprites = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", "PulseArm"));
        }

        public override bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {

            if (keyState.IsKeyDown(_pulseKey) && _pulseCoolDownCounter <= 0)
            {
                _firingPulseArm = true;
                if (teddy.PreviousRightOrLeft == Teddy.Direction.Left)
                {
                    screen.GameObjects.Add(new PulseProjectile(_game, new Vector2(teddy.Position.X + 10, teddy.Position.Y + 39), teddy, new Vector2(-_pulseVelocity, 0)));
                }
                else
                {
                    screen.GameObjects.Add(new PulseProjectile(_game, new Vector2(teddy.Position.X + 40, teddy.Position.Y + 39), teddy, new Vector2(_pulseVelocity, 0)));
                }

                _pulseCoolDownCounter = _pulseCoolDown;
                _drawFiringSprite = true;
            }
            else
            {
                _firingPulseArm = false;
                _pulseCoolDownCounter--;

                if (_drawFiringSprite == true)
                {
                    if (_pulseCoolDownCounter < _pulseCoolDown - 15)
                    {
                        _drawFiringSprite = false;
                    }
                }
            }
            return true;
        }

        public override bool BeforeDraw(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {
            if ( _drawFiringSprite)
            {
                teddy.SetSpriteState(Teddy.TeddySpriteState.Run3);

                if (teddy.Facing != Teddy.Direction.Left && teddy.Facing != Teddy.Direction.Right)
                {
                        teddy.Facing = teddy.PreviousRightOrLeft;
                }
            }

            return true;
        }

        public override void AfterDraw(GameTime gameTime, SpriteBatch batch, Teddy teddy, SpriteEffects seff)
        {

            if ( _drawFiringSprite)
            {
                batch.Draw(_pulseArmSprites, teddy.Position, new Rectangle(100, 0, 50, 75), Color.White, 0, Vector2.Zero, 1, seff, 0);
            }
            else 
            {
                if (teddy.Facing != Teddy.Direction.Left && teddy.Facing != Teddy.Direction.Right)
                {
                    batch.Draw(_pulseArmSprites, teddy.Position, new Rectangle(150, 0, 50, 75), Color.White, 0, Vector2.Zero, 1, seff, 0);
                }
                else
                {
                    batch.Draw(_pulseArmSprites, teddy.Position, teddy.GetBoxToDraw(), Color.White, 0, Vector2.Zero, 1, seff, 0);
                }

            }

        }

    }
}
