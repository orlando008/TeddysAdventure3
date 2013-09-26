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

        //Teddy Constant values
        private Vector2 _fallingBoxOffset = new Vector2(-15, 0);  //Offset to get Teddy's body to line up with his non falling self
        private Vector2 _blanketBoxOffset = new Vector2(5, 79 - 28);  //Height of blanket sprite - distance to teddys hands


        //Teddy State
        private TeddyModeEnum _teddyMode = TeddyModeEnum.Normal;




        private enum TeddyModeEnum
        {
            Normal = 0,
            Falling = 1,
            Parachuting = 2,
            FallingToDeath = 3
        }


        public TeddyFalling(Game game,  Vector2 initialPosition, Vector2 sizeOfFrame):base(game, initialPosition, sizeOfFrame)
        {
            _teddySprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyFall"));
            _blanketParachuteSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "Blanket"));
        }

        //Falling teddy is wider than normal teddy
        //Because of this, we must alter his position when changing modes otherwise it will appear like his body is jumping sideways
        private void ChangeTeddyMode(TeddyModeEnum mode)
        {
            switch (mode)
            {
                case TeddyModeEnum.Normal:
                    TERMINAL_VELOCITY = 10;
                    if (_teddyMode == TeddyModeEnum.Parachuting)
                        this.Position = this.Position - _fallingBoxOffset;

                    break;

                case TeddyModeEnum.Parachuting:
                    TERMINAL_VELOCITY = 4;
                    this.Position = this.Position + _fallingBoxOffset;
                    break;

                case TeddyModeEnum.Falling:

                    if (_teddyMode == TeddyModeEnum.Parachuting)
                        this.Position = this.Position - _fallingBoxOffset;

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

        protected override GeometryMethods.RectangleF TeddyRectangle
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

            //If after running the base update our y Velocity is zero, then we must be standing on something so we are in normel mode
            if (_teddyMode != TeddyModeEnum.Normal ) {
                if (_yVelocity == 0.0f)
                {
                    ChangeTeddyMode(TeddyModeEnum.Normal);
                }
            }

            if ( _teddyMode == TeddyModeEnum.Normal ){
                if (_yVelocity > 0.0f) {
                    //We have begun falling, start parachuting
                    ChangeTeddyMode(TeddyModeEnum.Parachuting);
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
                        ChangeTeddyMode(TeddyModeEnum.Falling);
                    }
                    else
                    {
                        ChangeTeddyMode(TeddyModeEnum.Parachuting);
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
                    base.Draw(gameTime, teddyBatch);
                    break;
            }

        }



        private void DrawParachuting(GameTime gameTime, SpriteBatch teddyBatch)
        {

            var teddyBox = new Rectangle(0, 0, _teddySprite.Width, _teddySprite.Height);
            var blanketBox = new Rectangle(0, 0, _blanketParachuteSprite.Width, _blanketParachuteSprite.Height);


            teddyBatch.Draw(_teddySprite,   this.Position ,  teddyBox, Color.White);
            teddyBatch.Draw(_blanketParachuteSprite, this.Position - _blanketBoxOffset , blanketBox, Color.White);

        }

    }
}
