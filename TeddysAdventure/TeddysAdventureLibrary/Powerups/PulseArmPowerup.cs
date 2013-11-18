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
        protected Texture2D _pulseArmChargingSprite;
        protected bool _charging = false;
        protected int _powerLevel = 0;
        protected int _powerLevelMax = 80;

        public PulseArmPowerup(Game game):base(game){
            _firingPulseArm = false;
            _pulseCoolDownCounter = 0;
            _pulseArmSprites = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", "PulseArm"));
            _pulseArmChargingSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "RunGlow"));
        }

        public override bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {
            if (_charging == true & keyState.IsKeyUp(_pulseKey))
            {
                _firingPulseArm = true;

                if (teddy.PreviousRightOrLeft == Teddy.Direction.Left)
                {
                    screen.GameObjects.Add(new PulseProjectile(_game, new Vector2(teddy.Position.X + 10, teddy.Position.Y + 39), teddy, new Vector2(-_pulseVelocity, 0), (float)(_powerLevel / 10), (_powerLevel / 5)));
                }
                else
                {
                    screen.GameObjects.Add(new PulseProjectile(_game, new Vector2(teddy.Position.X + 40, teddy.Position.Y + 39), teddy, new Vector2(_pulseVelocity, 0), (float)(_powerLevel / 10), (_powerLevel / 5)));
                }

                _pulseCoolDownCounter = _pulseCoolDown;
                _drawFiringSprite = true;
                _charging = false;
                _powerLevel = 0;
            }

            if (_charging)
            {
                if (_powerLevel < _powerLevelMax)
                {
                    _powerLevel++;
                }
            }

            if (keyState.IsKeyDown(_pulseKey) && _pulseCoolDownCounter <= 0)
            {
                _charging = true;
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

                int radius = _powerLevel/2;

                if (_charging && teddy.Facing == Teddy.Direction.Left)
                {
                    batch.Draw(_pulseArmChargingSprite, new Rectangle((int)teddy.Position.X - (radius/2), (int)teddy.Position.Y + 47 - (radius / 2), radius, radius), new Rectangle(0, 0, _pulseArmChargingSprite.Width, 12), Color.Blue);
                }
                else if (_charging && teddy.Facing == Teddy.Direction.Right)
                {
                    batch.Draw(_pulseArmChargingSprite, new Rectangle((int)teddy.Position.X + (int)teddy.FrameSize.X - (radius/2), (int)teddy.Position.Y + 47 - (radius/2), radius, radius), new Rectangle(0, 0, _pulseArmChargingSprite.Width, 12), Color.Blue);
                }

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
