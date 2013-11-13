using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary.Powerups
{
   public class BlanketPowerup: Powerup
    {

        //Teddy Resources
        private Texture2D _teddySprite;
        private Texture2D _blanketParachuteSprite;
        private Texture2D _blanketFallingSprite;
        private Rectangle _teddyBox;
        private Rectangle _blanketBox;

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



        public BlanketPowerup(Game game) :base(game)
        {
            _teddySprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyFall"));
            _blanketParachuteSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "Blanket"));
            _blanketFallingSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "BlanketFall"));
            _teddyBox   = new Rectangle(0, 0, _teddySprite.Width, _teddySprite.Height);
            _blanketBox = new Rectangle(0, 0, _blanketParachuteSprite.Width, _blanketParachuteSprite.Height);
        }

        //Falling teddy is wider than normal teddy
        //Because of this, we must alter his position when changing modes otherwise it will appear like his body is jumping sideways
        private void ChangeTeddyMode(TeddyModeEnum mode, Screen currentScreen, Teddy teddy)
        {
            switch (mode)
            {
                case TeddyModeEnum.Normal:
                    teddy.SetTerminalVelocity( 10);
                    if (_teddyMode != TeddyModeEnum.Normal)
                        teddy.movePlayerX(-_fallingBoxOffset, currentScreen);

                    break;

                case TeddyModeEnum.Parachuting:
                    teddy.SetTerminalVelocity(  2);
                    if (_teddyMode == TeddyModeEnum.Normal)
                        teddy.movePlayerX(_fallingBoxOffset, currentScreen);
                    break;

                case TeddyModeEnum.Falling:

                    if (_teddyMode == TeddyModeEnum.Normal)
                        teddy.movePlayerX(_fallingBoxOffset, currentScreen);

                    teddy.SetTerminalVelocity( 10);
                    break;

                case TeddyModeEnum.FallingToDeath:
                   teddy.SetTerminalVelocity(  int.MaxValue);
                    if (_teddyMode == TeddyModeEnum.Parachuting || _teddyMode == TeddyModeEnum.Falling)
                        teddy.movePlayerX(-_fallingBoxOffset, currentScreen);

                    break;

            }
            _teddyMode = mode;
        }



        
        private GeometryMethods.RectangleF BlanketRectangle(Teddy teddy)
        {

                Vector2 blanketPosition;
                switch (_teddyMode)
                {
                    case TeddyModeEnum.Parachuting:
                        blanketPosition = teddy.Position - _blanketBoxOffset + new Vector2(_blanketRotationOffset.X, 0f);
                        return new  GeometryMethods.RectangleF(blanketPosition.X, blanketPosition.Y, _blanketParachuteSprite.Width, _blanketParachuteSprite.Height);
                    case TeddyModeEnum.Falling:
                        blanketPosition = teddy.Position - _blanketFallingBoxOffset;
                        return new GeometryMethods.RectangleF(blanketPosition.X, blanketPosition.Y, _blanketFallingSprite.Width, _blanketFallingSprite.Height);
                }
                return null;
        }

        public override  GeometryMethods.RectangleF GetExpandedPowerupRectangle(Teddy teddy)
        {

                switch (_teddyMode)
                {
                    case TeddyModeEnum.Parachuting:
                    case TeddyModeEnum.Falling:
                        var fallingPosition = teddy.Position ;

                        return new GeometryMethods.RectangleF(fallingPosition.X, fallingPosition.Y, _teddySprite.Width, _teddySprite.Height); 

                }
                return teddy.JustTeddyRectangle;    
        }


        public bool _spacePressedDown = false;

        public override bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState) 
        {

            bool performTeddyUpdate = false;

            
            //If after running the base update our y Velocity is zero, then we must be standing on something so we are in normel mode
            if (_teddyMode != TeddyModeEnum.Normal && _teddyMode != TeddyModeEnum.FallingToDeath) {
                if ( teddy.Velocity.Y == 0.0f)
                {
                    ChangeTeddyMode(TeddyModeEnum.Normal, screen, teddy);
                }
            }

            if ( _teddyMode == TeddyModeEnum.Normal ){
                if (teddy.Velocity.Y > 0.0f) {
                    //We have begun falling, start parachuting
                    ChangeTeddyMode(TeddyModeEnum.Parachuting, screen, teddy);
                }
            }


            switch (_teddyMode)
            {
                case TeddyModeEnum.Normal:
                case TeddyModeEnum.FallingToDeath:
                    performTeddyUpdate = true;   
                /// base.Update(gameTime);            //Only if we have landed on something does teddy move normally
                    break;

                case TeddyModeEnum.Parachuting:
                case TeddyModeEnum.Falling:


                    if (keyState.IsKeyDown(Keys.Space) && !_spacePressedDown)
                    {
                        _spacePressedDown = true;
                        if (_teddyMode == TeddyModeEnum.Parachuting)
                            ChangeTeddyMode(TeddyModeEnum.Falling, screen, teddy);
                        else
                            ChangeTeddyMode(TeddyModeEnum.Parachuting, screen, teddy);
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


                    teddy.SetVelocity(new Vector2(- (_rotationAngle / _maxAbsoluteRotationAngle) * teddy.RunSpeed, teddy.Velocity.Y));
                                        

                    //Check if teddy hit anything
                    teddy.movePlayerAndCheckState(screen, keyState);

                    //Check to see if any enemies hit the blanket
                    if (_teddyMode == TeddyModeEnum.Parachuting || _teddyMode == TeddyModeEnum.Falling)
                        checkBlanketEnemies(screen, keyState, BlanketRectangle(teddy), teddy);

                    break;

            }
                    return performTeddyUpdate;
        }


        private void checkBlanketEnemies(Screen currentScreen, KeyboardState keyState, GeometryMethods.RectangleF playerRectangle, Teddy teddy)
        {
            foreach (Enemy e in currentScreen.Enemies)
            {
                checkBlanketEnemyInteractions(e, currentScreen, keyState, playerRectangle, teddy);

                if (e.ChildrenEnemies != null)
                {
                    foreach (Enemy e2 in e.ChildrenEnemies)
                    {
                        checkBlanketEnemyInteractions(e2, currentScreen, keyState, playerRectangle, teddy);
                    }
                }
            }
        }

        private void checkBlanketEnemyInteractions(Enemy e, Screen currentScreen, KeyboardState keyState, GeometryMethods.RectangleF playerRectangle, Teddy teddy)
        {
            GeometryMethods.RectangleF rHit = null;

            if (e.CanInteractWithPlayer & (e.RectangleInsersectsWithHitBoxes(playerRectangle, ref rHit)) )
            {
                //Blanket was hit!!!!!!
                ChangeTeddyMode(TeddyModeEnum.FallingToDeath, currentScreen, teddy);
            }
        }

       public override bool HandleLandingOnEnemy(Enemy e, KeyboardState keyState)
        {
            bool performBaseAction = false;
             switch (_teddyMode)
            {
                case TeddyModeEnum.FallingToDeath:
                    break;
                default:
                     performBaseAction  = true;
                   // base.HandleLandingOnEnemy(e, keyState);
                    break;
            }
            return performBaseAction;
        }

        public override bool HandleEnemyInteraction(Enemy e, Screen currentScreen, GeometryMethods.RectangleF enemyHitBox)
        {
            bool performBaseAction = false;
             switch (_teddyMode)
            {
                case TeddyModeEnum.FallingToDeath:
                    break;
                default:
                    performBaseAction = true;
                    break;
            }
            return performBaseAction;

        }


        public override  bool BeforeDraw(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {
            bool performTeddyUpdate = false;

            switch (_teddyMode)
            {
                case TeddyModeEnum.Normal:
                case TeddyModeEnum.FallingToDeath:
                    performTeddyUpdate = true;
                    break;
                case TeddyModeEnum.Parachuting:
                    DrawParachuting(gameTime, teddyBatch, teddy);
                    break;
                case TeddyModeEnum.Falling:
                    DrawFallingParachuting(gameTime, teddyBatch, teddy);
                    break;
            }

            return performTeddyUpdate;

        }


        private void DrawParachuting(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {


#if COLLISIONS
            Texture2D teddyFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            teddyFill.SetData<Color>(new Color[] { Color.Red });
            teddyBatch.Draw(teddyFill, teddy.Position, _teddyBox, Color.Red);

            Texture2D blanketFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blanketFill.SetData<Color>(new Color[] { Color.LimeGreen });
            teddyBatch.Draw(blanketFill, teddy.Position - _blanketBoxOffset + new Vector2((float)_blanketRotationOffset.X, 0f), _blanketBox, Color.LimeGreen);
#endif

            double rotationRadians = (float)_rotationAngle * Math.PI / 180;
            var originT = new Vector2(_teddyBox.Width / 2, _teddyBox.Height / 2);
            var originB = new Vector2(_blanketBox.Width / 2, _blanketBox.Height / 2);

            Vector2 tPosition = teddy.Position + originT;
            Vector2 bPosition = (Vector2)(teddy.Position + originT + _blanketRotationOffset);

            teddyBatch.Draw(_teddySprite, tPosition, null, Color.White, (float)rotationRadians, originT, 1, SpriteEffects.None, 0);
            teddyBatch.Draw(_blanketParachuteSprite, bPosition, null, Color.White, (float)rotationRadians, originB, 1, SpriteEffects.None, 0);

        }

        private void DrawFallingParachuting(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {

            var teddyBox = new Rectangle(0, 0, _teddySprite.Width, _teddySprite.Height);
            var blanketBox = new Rectangle(0, 0, _blanketFallingSprite.Width, _blanketFallingSprite.Height);
            var blanketDestinationBox = new Rectangle((int)(teddy.Position - _blanketFallingBoxOffset).X, (int)(teddy.Position - _blanketFallingBoxOffset).Y, _blanketFallingSprite.Width, _blanketFallingSprite.Height);

#if COLLISIONS
            Texture2D teddyFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            teddyFill.SetData<Color>(new Color[] { Color.Red });
            teddyBatch.Draw(teddyFill, teddy.Position, teddyBox, Color.Red);

            Texture2D blanketFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blanketFill.SetData<Color>(new Color[] { Color.LimeGreen });
            teddyBatch.Draw(blanketFill, teddy.Position - _blanketFallingBoxOffset, blanketBox, Color.LimeGreen);
#endif


            teddyBatch.Draw(_teddySprite, teddy.Position, teddyBox, Color.White);

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
