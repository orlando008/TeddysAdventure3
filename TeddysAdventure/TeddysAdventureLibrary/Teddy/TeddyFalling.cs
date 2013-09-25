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

        //Teddy State
        private TeddyModeEnum _teddyMode;




        private enum TeddyModeEnum
        {
            Normal = 0,
            Falling = 1,
            Parachuting = 2
        }


        public TeddyFalling(Game game,  Vector2 initialPosition, Vector2 sizeOfFrame):base(game, initialPosition, sizeOfFrame)
        {
            TERMINAL_VELOCITY = 4;



        }



        public override void Update(GameTime gameTime) 
        {
            base.Update(gameTime);

            //If after running the base update our y Velocity is zero, then we must be standing on something so we are in normel mode
            if (_teddyMode != TeddyModeEnum.Normal ) {
                if (_yVelocity == 0.0f)
                {
                    _teddyMode = TeddyModeEnum.Normal;
                }
            }

            if ( _teddyMode == TeddyModeEnum.Normal ){
                if (_yVelocity > 0.0f) {
                    //We have begun falling, start parachuting
                    _teddyMode = TeddyModeEnum.Parachuting;
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
                        _teddyMode = TeddyModeEnum.Falling;
                        TERMINAL_VELOCITY = 10;
                    }
                    else
                    {
                        _teddyMode = TeddyModeEnum.Parachuting;
                        TERMINAL_VELOCITY = 4;
                    }
                }


                if (keyState.IsKeyUp(Keys.Space))
                {
                    _spacePressedDown = false;
                }

            }


        }


        public override void Draw(GameTime gameTime)
        {
            //todo: when we are falling we will do different drawing here
            switch (_teddyMode)
            {
                case TeddyModeEnum.Normal:
                    base.Draw(gameTime);
                    break;
                case TeddyModeEnum.Parachuting:
                    base.Draw(gameTime);
                    break;
                case TeddyModeEnum.Falling:
                    base.Draw(gameTime);
                    break;
            }

        }

    }
}
