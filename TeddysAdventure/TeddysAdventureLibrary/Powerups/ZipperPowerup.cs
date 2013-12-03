using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary
{
    class ZipperPowerup : Powerup
    {
        protected Texture2D _unzipAnimation;
        protected Texture2D _fluffSprite;
        protected Keys _unzipKey = Keys.Z;
        protected bool _startUnzip;
        protected bool _unzipping;
        protected bool _zipping = false;
        private Rectangle _fluffBox;

        private FluffPowerup _fluffPowerup;

        protected List<Rectangle> _animationRectangles = new List<Rectangle>();
        protected List<int> _animationFrameCount = new List<int>();
        private int _currentAnimationRectangle = 0;
        private int _currentAnimationFrameCount = 0;

        public ZipperPowerup(Game game, Teddy teddy, bool startZipping)
            : base(game)
        {
            _unzipAnimation = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "UnzipAnimation"));
            _fluffSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", "Fluff"));
            _fluffBox = new Rectangle(0, 0, _fluffSprite.Width / 2, _fluffSprite.Height / 2);

            _animationRectangles.Add(new Rectangle(0, 0, 50, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(50, 0, 50, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(100, 0, 50, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(150, 0, 62, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(213, 0, 103, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(317, 0, 139, 75));
            _animationFrameCount.Add(15);

            _fluffPowerup = new FluffPowerup(game, teddy);

            if (startZipping)
            {
                _zipping = true ;
                _currentAnimationRectangle = _animationRectangles.Count - 1;
                _currentAnimationFrameCount = _animationFrameCount.Last<int>();
                teddy.SetSpriteState(Teddy.TeddySpriteState.BlinkNoArms);
                _fluffPowerup.ResetFluff(teddy);
            }

        }

        public override bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {
            if (!_zipping)
            {
                if (teddy.Velocity.Y == 0 && teddy.Velocity.X == 0)
                {
                    if (_startUnzip == true && _unzipping == false && keyState.IsKeyUp(_unzipKey))
                    {
                        _unzipping = true;
                        teddy.SetSpriteState(Teddy.TeddySpriteState.BlinkNoArms);
                        _fluffPowerup.ResetFluff(teddy);
                    }

                    if (_startUnzip == false && keyState.IsKeyDown(_unzipKey))
                    {
                        _startUnzip = true;
                        _currentAnimationRectangle = 0;
                        _currentAnimationFrameCount = 0;
                    }
                }
            }

            //If teddy is (un)zipping, we do not want him to be able to move, so we cancel his update in that case
            if (_zipping || _unzipping)
                return false;
            else
                return base.Update(gameTime, screen, teddy, keyState);
        }

        public override bool BeforeDraw(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {
            if (_unzipping)
            {
                _fluffPowerup.DrawFluff(teddyBatch, gameTime);
                teddyBatch.Draw(_unzipAnimation, new Vector2(teddy.Position.X - ((_animationRectangles[_currentAnimationRectangle].Width - teddy.FrameSize.X)/2), teddy.Position.Y), _animationRectangles[_currentAnimationRectangle], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                _currentAnimationFrameCount++;

                if (_currentAnimationFrameCount >= _animationFrameCount[_currentAnimationRectangle])
                {
                    _currentAnimationRectangle++;
                    _currentAnimationFrameCount = 0;
                }

                if (_currentAnimationRectangle >= _animationRectangles.Count)
                {
                    _startUnzip = false;
                    _unzipping = false;

                    teddy.SetSpriteState(Teddy.TeddySpriteState.Blink1);
                    _fluffPowerup.ResetFluff(teddy);

                    teddy.SetPowerup(_fluffPowerup);
                }
                return false;
            } else if (_zipping){

                _fluffPowerup.DrawFluff(teddyBatch, gameTime);
                teddyBatch.Draw(_unzipAnimation, new Vector2(teddy.Position.X - ((_animationRectangles[_currentAnimationRectangle].Width - teddy.FrameSize.X)/2), teddy.Position.Y), _animationRectangles[_currentAnimationRectangle], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                _currentAnimationFrameCount--;

                if (_currentAnimationFrameCount < 0)
                {
                    _currentAnimationRectangle--;

                    if (_currentAnimationRectangle < 0)
                    {
                        _startUnzip = false;
                        _unzipping = false;
                        _zipping = false;
                    }
                    else
                    {
                        _currentAnimationFrameCount = _animationFrameCount[_currentAnimationRectangle];
                    }
                }

                return false;
            }

            return true;
        }

    }
}
