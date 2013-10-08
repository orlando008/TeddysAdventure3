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
        private Rectangle _teddyBox;
        private Rectangle _blanketBox;

        private Game _game;

        //Teddy Constant values

        private Vector2 _fallingBoxOffset = new Vector2(-19, 0);  //Offset to get Teddy's body to line up with his non falling self
        private Vector2 _blanketBoxOffset = new Vector2(5, 79 - 28);  //Height of blanket sprite - distance to teddys hands
        private Vector2 _blanketFallingBoxOffset = new Vector2(25, 155 - 28); //X = Teddy's left hand postion, Y = Height of blanket prite - distance to teddy's left hand
        private float _maxAbsoluteRotationAngle = 30;
        private float _rotationTickAngle = .5f;

        //Teddy State
        private TeddyModeEnum _teddyMode = TeddyModeEnum.Normal;
        private float _rotationAngle = 0.0f;


        private Vector2 _blanketRotationOffset = new Vector2(0, 0);

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
            _teddyBox   = new Rectangle(0, 0, _teddySprite.Width, _teddySprite.Height);
            _blanketBox = new Rectangle(0, 0, _blanketParachuteSprite.Width, _blanketParachuteSprite.Height);
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
                    if (_teddyMode != TeddyModeEnum.Normal)
                        movePlayerX(-_fallingBoxOffset, currentScreen);

                    break;

                case TeddyModeEnum.Parachuting:
                    TERMINAL_VELOCITY = 2;
                    if ( _teddyMode == TeddyModeEnum.Normal )
                        movePlayerX(_fallingBoxOffset, currentScreen);
                    break;

                case TeddyModeEnum.Falling:

                    if (_teddyMode == TeddyModeEnum.Normal)
                        movePlayerX(_fallingBoxOffset, currentScreen);

                    TERMINAL_VELOCITY = 10;
                    break;

                case TeddyModeEnum.FallingToDeath:
                    TERMINAL_VELOCITY = int.MaxValue;
                    if (_teddyMode == TeddyModeEnum.Parachuting || _teddyMode == TeddyModeEnum.Falling)
                        movePlayerX(-_fallingBoxOffset, currentScreen);

                    break;

            }
            _teddyMode = mode;
        }


        private GeometryMethods.RectangleF BlanketRectangle
        {
            get 
            {
                Vector2 blanketPosition;
                switch (_teddyMode)
                {
                    case TeddyModeEnum.Parachuting:
                        blanketPosition = this.Position - _blanketBoxOffset + new Vector2(_blanketRotationOffset.X, 0f);
                        return new  GeometryMethods.RectangleF(blanketPosition.X, blanketPosition.Y, _blanketParachuteSprite.Width, _blanketParachuteSprite.Height);
                    case TeddyModeEnum.Falling:
                        blanketPosition = this.Position - _blanketFallingBoxOffset;
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
                    case TeddyModeEnum.Falling:
                        var fallingPosition = this.Position ;

                        return new GeometryMethods.RectangleF(fallingPosition.X, fallingPosition.Y, _teddySprite.Width, _teddySprite.Height); 

                }
                return base.TeddyRectangle;    
            }
        }


        public override void Update(GameTime gameTime) 
        {

            Screen currentScreen = (Screen)_game.Components[0];
            
            //If after running the base update our y Velocity is zero, then we must be standing on something so we are in normel mode
            if (_teddyMode != TeddyModeEnum.Normal && _teddyMode != TeddyModeEnum.FallingToDeath) {
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


            switch (_teddyMode)
            {
                case TeddyModeEnum.Normal:
                case TeddyModeEnum.FallingToDeath:
                    base.Update(gameTime);            //Only if we have landed on something does teddy move normally
                    break;

                case TeddyModeEnum.Parachuting:
                case TeddyModeEnum.Falling:

                    KeyboardState keyState = Keyboard.GetState();

                    if (keyState.IsKeyDown(Keys.Space) && !_spacePressedDown)
                    {
                        _spacePressedDown = true;
                        if (_teddyMode == TeddyModeEnum.Parachuting)
                            ChangeTeddyMode(TeddyModeEnum.Falling, currentScreen);
                        else
                            ChangeTeddyMode(TeddyModeEnum.Parachuting, currentScreen);
                    }


                    if (keyState.IsKeyUp(Keys.Space))
                    {
                        _spacePressedDown = false;
                    }

                    
                    if (keyState.IsKeyDown(Keys.Left))
                        _rotationAngle = Math.Min(_rotationAngle + _rotationTickAngle, _maxAbsoluteRotationAngle);
                    else if (keyState.IsKeyDown(Keys.Right))
                        _rotationAngle = Math.Max(_rotationAngle - _rotationTickAngle, -_maxAbsoluteRotationAngle);
                    else
                        _rotationAngle = _rotationAngle - Math.Sign(_rotationAngle) * _rotationTickAngle;


                    double r = _teddyBox.Height / 2 + _blanketBoxOffset.Y - _blanketBox.Height / 2;
                    double dx = Math.Cos((_rotationAngle + 90)  * Math.PI / 180)* r;
                    double dy = Math.Sin((_rotationAngle+90) * Math.PI / 180) * r;
                    _blanketRotationOffset = new Vector2((float)-dx, (float)-dy);


                    _playerOverallVelocity.X = - (_rotationAngle / _maxAbsoluteRotationAngle) * this.runSpeed;

                    //Check if teddy hit anything
                    movePlayerAndCheckState(currentScreen, keyState);

                    //Check to see if any enemies hit the blanket
                    if (_teddyMode == TeddyModeEnum.Parachuting || _teddyMode == TeddyModeEnum.Falling)
                        checkBlanketEnemies(currentScreen, keyState, this.BlanketRectangle);

                    break;

            }

        }


        private void checkBlanketEnemies(Screen currentScreen, KeyboardState keyState, GeometryMethods.RectangleF playerRectangle)
        {
            foreach (Enemy e in currentScreen.Enemies)
            {
                checkBlanketEnemyInteractions(e, currentScreen, keyState, playerRectangle);

                if (e.ChildrenEnemies != null)
                {
                    foreach (Enemy e2 in e.ChildrenEnemies)
                    {
                        checkBlanketEnemyInteractions(e2, currentScreen, keyState, playerRectangle);
                    }
                }
            }
        }

        private void checkBlanketEnemyInteractions(Enemy e, Screen currentScreen, KeyboardState keyState, GeometryMethods.RectangleF playerRectangle)
        {
            GeometryMethods.RectangleF rHit = null;

            if (e.CanInteractWithPlayer & (e.RectangleInsersectsWithHitBoxes(playerRectangle, ref rHit)) )
            {
                //Blanket was hit!!!!!!
                ChangeTeddyMode(TeddyModeEnum.FallingToDeath, currentScreen);
            }
        }

        protected override void HandleLandingOnEnemy(Enemy e, KeyboardState keyState)
        {
             switch (_teddyMode)
            {
                case TeddyModeEnum.FallingToDeath:
                    break;
                default:
                    base.HandleLandingOnEnemy(e, keyState);
                    break;
            }
        }

        protected override void HandleEnemyInteraction(Enemy e, Screen currentScreen, GeometryMethods.RectangleF enemyHitBox)
        {
             switch (_teddyMode)
            {
                case TeddyModeEnum.FallingToDeath:
                    break;
                default:
                    base.HandleEnemyInteraction(e, currentScreen, enemyHitBox);
                    break;
            }

        } 

        public override void Draw(GameTime gameTime, SpriteBatch teddyBatch)
        {
            //todo: when we are falling we will do different drawing here
            switch (_teddyMode)
            {
                case TeddyModeEnum.Normal:
                case TeddyModeEnum.FallingToDeath:
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


#if COLLISIONS
            Texture2D teddyFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            teddyFill.SetData<Color>(new Color[] { Color.Red });
            teddyBatch.Draw(teddyFill, this.Position, _teddyBox, Color.Red);
                        
            Texture2D blanketFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blanketFill.SetData<Color>(new Color[] { Color.LimeGreen });
            teddyBatch.Draw(blanketFill, this.Position - _blanketBoxOffset + new Vector2((float) _blanketRotationOffset.X,0f), _blanketBox, Color.LimeGreen);
#endif
            
            double rotationRadians = (float)_rotationAngle * Math.PI / 180;
            var originT = new Vector2(_teddyBox.Width / 2, _teddyBox.Height / 2);
            var originB = new Vector2(_blanketBox.Width / 2, _blanketBox.Height / 2);

            Vector2 tPosition =  this.Position + originT;
            Vector2 bPosition = (Vector2)( this.Position + originT + _blanketRotationOffset);     
     
            teddyBatch.Draw(_teddySprite, tPosition, null, Color.White, (float)rotationRadians, originT, 1, SpriteEffects.None, 0);
            teddyBatch.Draw(_blanketParachuteSprite, bPosition , null, Color.White, (float)rotationRadians, originB, 1, SpriteEffects.None, 0);

        }

        private void DrawFallingParachuting(GameTime gameTime, SpriteBatch teddyBatch)
        {

            var teddyBox = new Rectangle(0, 0, _teddySprite.Width, _teddySprite.Height);
            var blanketBox = new Rectangle(0, 0, _blanketFallingSprite.Width, _blanketFallingSprite.Height);
            var blanketDestinationBox = new Rectangle((int)(this.Position - _blanketFallingBoxOffset).X, (int)(this.Position - _blanketFallingBoxOffset).Y, _blanketFallingSprite.Width, _blanketFallingSprite.Height);

#if COLLISIONS
            Texture2D teddyFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            teddyFill.SetData<Color>(new Color[] { Color.Red });
            teddyBatch.Draw(teddyFill, this.Position, teddyBox, Color.Red);

            Texture2D blanketFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blanketFill.SetData<Color>(new Color[] { Color.LimeGreen });
            teddyBatch.Draw(blanketFill, this.Position - _blanketFallingBoxOffset , blanketBox, Color.LimeGreen);
#endif


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
