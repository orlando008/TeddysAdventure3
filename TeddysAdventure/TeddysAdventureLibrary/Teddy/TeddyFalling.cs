using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary
{
    class TeddyFalling : Teddy
    {

        //Teddy Resources
        private Texture2D _teddySprite;
        private Texture2D _blanketParachuteSprite;
        private Texture2D _blanketFallingSprite;

        private Game _game;

        //Teddy Constant values
        private Vector2 _fallingBoxOffset = new Vector2(-19, 0);  //Offset to get Teddy's body to line up with his non falling self
        private Vector2 _blanketBoxOffset = new Vector2(5, 79 - 28);  //Height of blanket sprite - distance to teddys hands
        private Vector2 _blanketFallingBoxOffset = new Vector2(25, 155 - 28); //X = Teddy's left hand postion, Y = Height of blanket prite - distance to teddy's left hand

        //Teddy State
        private TeddyModeEnum _teddyMode = TeddyModeEnum.Normal;

        private int _blanketFlappingCounter = 0;
        private int _blanketFlapInterval = 25;


        private enum TeddyModeEnum
        {
            Normal = 0,
            Falling = 1,
            Parachuting = 2,
            FallingToDeath = 3
        }


        public TeddyFalling(Game game,  Vector2 initialPosition, Vector2 sizeOfFrame):base(game,  initialPosition, sizeOfFrame)
        {
            _teddySprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyFall"));
            _blanketParachuteSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "Blanket"));
            _blanketFallingSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "BlanketFall"));
	    _game = game;
        }

        //Falling teddy is wider than normal teddy
        //Because of this, we must alter his position when changing modes otherwise it will appear like his body is jumping sideways
        private void ChangeTeddyMode(TeddyModeEnum mode, Screen currentScreen)
        {
            switch (mode)
            {
                case TeddyModeEnum.Normal:
                    TERMINAL_VELOCITY = 10;
                    if (_teddyMode == TeddyModeEnum.Parachuting)
                        movePlayerX(-_fallingBoxOffset, currentScreen);

                    break;

                case TeddyModeEnum.Parachuting:
                    TERMINAL_VELOCITY = 4;
                    movePlayerX(_fallingBoxOffset, currentScreen);
                    break;

                case TeddyModeEnum.Falling:

                    if (_teddyMode == TeddyModeEnum.Parachuting)
                        movePlayerX(-_fallingBoxOffset, currentScreen);

                    TERMINAL_VELOCITY = 10;
                    break;
            }
            _teddyMode = mode;
        }


        private GeometryMethods.RectangleF BlanketRectangle
        {
            get 
            {
                switch (_teddyMode)
                {
                    case TeddyModeEnum.Parachuting:
                        var blanketPosition = this.Position +  _blanketBoxOffset;
                        return new  GeometryMethods.RectangleF(blanketPosition.X, blanketPosition.Y, _blanketParachuteSprite.Width, _blanketParachuteSprite.Height); 
                        
                }
                return null;
            }
        }

        private GeometryMethods.RectangleF BlanketFallRectangle
        {
            get
            {
                switch (_teddyMode)
                {
                    case TeddyModeEnum.Falling:
                        var blanketPosition = this.Position + _blanketFallingBoxOffset;
                        return new GeometryMethods.RectangleF(blanketPosition.X, blanketPosition.Y, _blanketFallingSprite.Width, _blanketFallingSprite.Height);

                }
                return null;
            }
        }

        public override GeometryMethods.RectangleF TeddyRectangle
        {
            get
            {
                switch (_teddyMode)
                {
                    case TeddyModeEnum.Parachuting:
                        var fallingPosition = this.Position ;

                        return new GeometryMethods.RectangleF(fallingPosition.X, fallingPosition.Y, _teddySprite.Width, _teddySprite.Height); 

                }
                return base.TeddyRectangle;    
            }
        }


        public override void Update(GameTime gameTime) 
        {
            base.Update(gameTime);

            Screen currentScreen = (Screen)_game.Components[0];

            //If after running the base update our y Velocity is zero, then we must be standing on something so we are in normel mode
            if (_teddyMode != TeddyModeEnum.Normal ) {
                if (_yVelocity == 0.0f)
                {
                    ChangeTeddyMode(TeddyModeEnum.Normal, currentScreen);
                }
            }

            if ( _teddyMode == TeddyModeEnum.Normal ){
                if (_yVelocity > 0.0f) {
                    //We have begun falling, start parachuting
                    ChangeTeddyMode(TeddyModeEnum.Parachuting, currentScreen);
                }
            }

            //Falling/Parachute mode handling
            if (_teddyMode != TeddyModeEnum.Normal)
            {

                KeyboardState keyState = Keyboard.GetState();

                if (keyState.IsKeyDown(Keys.Space) && !_spacePressedDown)
                {
                    _spacePressedDown = true;
                    if (_teddyMode == TeddyModeEnum.Parachuting)
                    {
                        ChangeTeddyMode(TeddyModeEnum.Falling, currentScreen);
                    }
                    else
                    {
                        ChangeTeddyMode(TeddyModeEnum.Parachuting, currentScreen);
                    }
                }


                if (keyState.IsKeyUp(Keys.Space))
                {
                    _spacePressedDown = false;
                }





            }


        }


        

        public override void Draw(GameTime gameTime, SpriteBatch teddyBatch)
        {
            //todo: when we are falling we will do different drawing here
            switch (_teddyMode)
            {
                case TeddyModeEnum.Normal:
                    base.Draw(gameTime, teddyBatch);
                    break;
                case TeddyModeEnum.Parachuting:
                    DrawParachuting(gameTime, teddyBatch);
                    break;
                case TeddyModeEnum.Falling:
                    DrawFallingParachuting(gameTime, teddyBatch);
                    break;
            }

        }



        private void DrawParachuting(GameTime gameTime, SpriteBatch teddyBatch)
        {

            var teddyBox = new Rectangle(0, 0, _teddySprite.Width, _teddySprite.Height);
            var blanketBox = new Rectangle(0, 0, _blanketParachuteSprite.Width, _blanketParachuteSprite.Height);


            //Texture2D fill  = new Texture2D(_game.GraphicsDevice, 1,1, false, SurfaceFormat.Color);
            //fill.SetData<Color>( new Color[] { Color.Red });

            //teddyBatch.Draw(fill, this.Position, teddyBox, Color.Black); 
             


            teddyBatch.Draw(_teddySprite,   this.Position ,  teddyBox, Color.White);
            teddyBatch.Draw(_blanketParachuteSprite, this.Position - _blanketBoxOffset , blanketBox, Color.White);

        }

        private void DrawFallingParachuting(GameTime gameTime, SpriteBatch teddyBatch)
        {

            var teddyBox = new Rectangle(0, 0, _teddySprite.Width, _teddySprite.Height);
            var blanketBox = new Rectangle(0, 0, _blanketFallingSprite.Width, _blanketFallingSprite.Height);
            var blanketDestinationBox = new Rectangle((int)(this.Position - _blanketFallingBoxOffset).X, (int)(this.Position - _blanketFallingBoxOffset).Y, _blanketFallingSprite.Width, _blanketFallingSprite.Height);
 

            teddyBatch.Draw(_teddySprite, this.Position, teddyBox, Color.White);

            _blanketFlappingCounter++;

            SpriteEffects effect = SpriteEffects.FlipHorizontally;

            if (_blanketFlappingCounter < _blanketFlapInterval)
            {
                effect = SpriteEffects.None;
            }
            else if (_blanketFlappingCounter < _blanketFlapInterval * 2)
            {
                effect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                _blanketFlapInterval = new Random().Next(5, 25);
                _blanketFlappingCounter = 0;
            }

            teddyBatch.Draw(_blanketFallingSprite, blanketDestinationBox, blanketBox, Color.White, 0, Vector2.Zero, effect, 0);

        }

    }
}
