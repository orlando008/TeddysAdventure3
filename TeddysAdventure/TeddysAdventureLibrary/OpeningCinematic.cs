using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary
{
    public class OpeningCinematic : DrawableGameComponent
    {
        private Texture2D _textureForAnimation;
        private Texture2D _teddyTexture;
        public static SpriteBatch _spriteBatch;
        private int _frameCount;
        private Rectangle _boxToDraw;
        private bool _playerIsReady;
        private Teddy _teddy;
        private SpriteFont _hudFont;
        private bool _theQuestionHasBeenAsked;
        private bool _playerIsGoodToGo;
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
            _teddy = new Teddy(game, game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyRun")), new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 254, (Game.GraphicsDevice.Viewport.Height / 2) - _boxToDraw.Height), new Vector2(50, 75));
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _boxToDraw = new Rectangle(0, 0, 254, _textureForAnimation.Height);
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\HudFont");
        }


        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_textureForAnimation, new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 254, (Game.GraphicsDevice.Viewport.Height / 2)- _boxToDraw.Height), _boxToDraw, Color.White);
            if (_teddy.Position.X >= (Game.GraphicsDevice.Viewport.Width / 2) && _playerIsGoodToGo == false)
            {
                _spriteBatch.DrawString(_hudFont, "Let's do this?", new Vector2(_teddy.Position.X + _teddy.BoxToDraw.Width, _teddy.Position.Y), Color.White);
                _theQuestionHasBeenAsked = true;
            }
            else if(_playerIsGoodToGo)
            {
                _spriteBatch.DrawString(_hudFont, "Aight den", new Vector2(_teddy.Position.X + _teddy.BoxToDraw.Width, _teddy.Position.Y), Color.White);
            }

            _spriteBatch.End();


            if (_playerIsReady)
            {
                _teddy.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || _playerIsReady)
            {
                if (_playerIsGoodToGo)
                {
                    _frameCount++;
                    if (_frameCount > 100)
                    {
                        _finishedCinematic = true;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Space) && _theQuestionHasBeenAsked)
                {
                    _playerIsGoodToGo = true;
                    _frameCount = 0;
                }
                else
                {

                    _playerIsReady = true;
                    _boxToDraw = new Rectangle(254 * 3, 0, 254, _textureForAnimation.Height);

                    if (_teddy.Position.X < (Game.GraphicsDevice.Viewport.Width / 2))
                    {
                        _teddy.Position = new Vector2(_teddy.Position.X + 3, _teddy.Position.Y);
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
