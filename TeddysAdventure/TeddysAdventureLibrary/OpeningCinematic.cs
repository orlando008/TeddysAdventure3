using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TeddysAdventureLibrary
{
    public class OpeningCinematic : DrawableGameComponent
    {
        private Texture2D _textureForAnimation;
        private Texture2D _teddyTexture;
        public static SpriteBatch _spriteBatch;
        private int _frameCount;
        private Rectangle _boxToDraw;
        private bool _playerIsReadyToContinue;
        private Teddy _teddy;
        private SpriteFont _hudFont;
        private bool _theQuestionHasBeenAsked;
        private bool _playerIsDoneWithCinematic;
        private bool _finishedCinematic;
        private int _lengthOfPose = 10;

        public bool FinishedCinematic
        {
            get { return _finishedCinematic; }
        }

        public OpeningCinematic(Game game)
            : base(game)
        {
            _textureForAnimation = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Cinematics", "Dryer"));
            _teddy = new Teddy(game, new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 254, (Game.GraphicsDevice.Viewport.Height / 2) - _boxToDraw.Height), new Vector2(50, 75));
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _boxToDraw = new Rectangle(0, 0, 254, _textureForAnimation.Height);
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\Arial12");
        }


        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_textureForAnimation, new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 254, (Game.GraphicsDevice.Viewport.Height / 2)- _boxToDraw.Height), _boxToDraw, Color.White);
            if (_teddy.Position.X >= (Game.GraphicsDevice.Viewport.Width / 2) && _playerIsDoneWithCinematic == false)
            {
                _spriteBatch.DrawString(_hudFont, "Let's do this?", new Vector2(_teddy.Position.X + _teddy.FrameSize.X, _teddy.Position.Y), Color.White);
                _theQuestionHasBeenAsked = true;
            }
            else if(_playerIsDoneWithCinematic)
            {
                _spriteBatch.DrawString(_hudFont, "Aight den", new Vector2(_teddy.Position.X + _teddy.FrameSize.X, _teddy.Position.Y), Color.White);
            }
                        
            if (_playerIsReadyToContinue)
            {
                _teddy.Draw(gameTime, _spriteBatch);
            }
            _spriteBatch.End();

        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || _playerIsReadyToContinue)
            {
                if (_playerIsDoneWithCinematic)
                {
                    _frameCount++;
                    if (_frameCount > 100)
                    {
                        _finishedCinematic = true;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Space) && _theQuestionHasBeenAsked)
                {
                    _playerIsDoneWithCinematic = true;
                    _frameCount = 0;
                }
                else
                {
                    if (_playerIsReadyToContinue == false)
                    {
                        //TODO: Change this sound to be like a dryer door popping open.
                        SoundEffectHelper.PlaySoundEffect(Game, "GoodScan");
                    }
                        
                    _playerIsReadyToContinue = true;
                    _boxToDraw = new Rectangle(254 * 3, 0, 254, _textureForAnimation.Height);

                    if (_teddy.Position.X < (Game.GraphicsDevice.Viewport.Width / 2))
                    {
                        _teddy.Position = new Vector2(_teddy.Position.X + 3, _teddy.Position.Y);
                        _teddy.MoveRight(3);
                        _teddy.SetFacingSprite();
                    }

                    
                }
               
            }
            else
            {
                if (_frameCount < _lengthOfPose)
                {
                    _boxToDraw = new Rectangle(0, 0, 254, _textureForAnimation.Height);
                }
                else if (_frameCount < _lengthOfPose * 2)
                {
                    _boxToDraw = new Rectangle(254, 0, 254, _textureForAnimation.Height);
                }
                else if (_frameCount < _lengthOfPose * 4)
                {
                    _boxToDraw = new Rectangle(254 * 2, 0, 254, _textureForAnimation.Height);
                    _frameCount = 0;
                }
                _frameCount++;
            }

          
        }
    }
}
