﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary
{
    public class Teddy : DrawableGameComponent
    {
        public static SpriteBatch spriteBatch;

        public enum Direction
        {
            Up = 0, Down, Left, Right
        }

        public enum TeddySpriteState
        {
            Run1= 1,
            Run2 = 2,
            Run3 = 3,
            Blink1 = 4,
            Blink2 = 5,
            Blink3 = 6,
            Blink4 = 7,
            BlinkNoArms = 8
        }

        protected int TERMINAL_VELOCITY = 10;

        private int _x;
        private int _y;
        private int _height;
        private int _width;
        protected Texture2D _styleSheet;
        private Texture2D _runGlow;
        private Game _game;
        protected Vector2 _position;
        private Rectangle _collisionBox;
        private Rectangle _boxToDraw;
        private Vector2 _frameSize;
        protected int walkSpeed = 2;
        private int runSpeed = 3;
        private int _damage = 1;

        public int RunSpeed
        {
            get { return runSpeed; }
        }

        private int _facingCounter = 0;
        private int _poseLengthWalk = 15;
        private int _initialBlinkWaitCounter = 500;
        private int _initialBlinkWait = 500;
        private int _blinkPoseLength = 10;
        protected TeddySpriteState _currentSprite = TeddySpriteState.Run1;
        private Direction _facing = Direction.Up;
        private Direction _previousRightOrLeft = Direction.Right;


        private bool _dead = false;
        private bool _isRunning = false;

        private bool _isJumping = false;

        public bool IsJumping
        {
            get { return _isJumping; }
            set { _isJumping = value; }
        }
        private float _initialJumpVelocity = 10;
        protected float _gravity = .25f;
        protected float _yVelocity = 0.0f;
        protected Vector2 _playerOverallVelocity;

        protected int _currentFluff = 100;
        private int _currentLives = 10;

        private int _enemiesDestroyed = 0;
        private ISurfaceInterface _ridingSurface = null;

        private int _recoverCounter = 0;
        private int _recoverWait = 100;
        private bool _isHit = false;

        public bool IsHit
        {
            get { return _isHit; }
            set { _isHit = value; }
        }
        
 
        private bool _levelComplete = false;

        //Keystate 
        protected bool _spacePressedDown = false;

        public Powerup _currentPowerup = null;

        public void Initialize()
        {
            
        }

        public Teddy(Game game,  Vector2 initialPosition, Vector2 sizeOfFrame)
            :base(game)
        {

            this.StyleSheet = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyRun"));
            _runGlow = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "RunGlow"));

            Game = game;
            Position = initialPosition;
            FrameSize = sizeOfFrame;

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        #region " Properties "
        public bool SpacePressed { get { return _spacePressedDown; } }

        public bool Dead
        {
            get { return _dead; }
            set { _dead = value; }
        }

        public bool LevelComplete
        {
            get { return _levelComplete; }
            set { _levelComplete = value; }
        }

        public int CurrentFluff
        {
            get { return _currentFluff; }
            set { _currentFluff = value; }
        }

        public int CurrentLives
        {
            get { return _currentLives; }
            set { _currentLives = value; }
        }

        public int EnemiesDestroyed
        {
            get { return _enemiesDestroyed; }
            set { _enemiesDestroyed = value; }
        }

        public ISurfaceInterface RidingSurface { get { return _ridingSurface; } }

        public Direction PreviousRightOrLeft
        {
            get { return _previousRightOrLeft; }
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public Texture2D StyleSheet
        {
            get { return _styleSheet; }
            set { _styleSheet = value; }
        }

        public Game Game
        {
            get { return _game; }
            set { _game = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 FrameSize
        {
            get { return _frameSize; }
            set { _frameSize = value; }
        }

        public Rectangle CollisionBox
        {
            get { return _collisionBox; }
            set { _collisionBox = value; }
        }

        public Direction Facing
        {
            get { return _facing; }
            set
            {
                _facing = value;
                if (_facing == Direction.Left || _facing == Direction.Right)
                {
                    _previousRightOrLeft = _facing;
                }
            }
        }

        public bool WearingPowerup
        {
            get 
            {
                if (_currentPowerup != null )
                    return true;
                else
                    return false;
            }

        }

        public void SetSpriteState(TeddySpriteState state)
        {
            _currentSprite = state;
        }

        public Rectangle? GetBoxToDraw()
        {


            switch (_currentSprite)
            {
                case TeddySpriteState.Run1:
                    return new Rectangle(0, 0, (int)FrameSize.X, (int)FrameSize.Y);
                case TeddySpriteState.Run2:
                    return new Rectangle(50, 0, (int)FrameSize.X, (int)FrameSize.Y);
                case TeddySpriteState.Run3:
                    return new Rectangle(100, 0, (int)FrameSize.X, (int)FrameSize.Y);
                case TeddySpriteState.Blink1:
                    return  new Rectangle(0, 75, (int)FrameSize.X, (int)FrameSize.Y);
                case TeddySpriteState.Blink2:
                    return new Rectangle(50, 75, (int)FrameSize.X, (int)FrameSize.Y);
                case TeddySpriteState.Blink3:
                    return new Rectangle(100, 75, (int)FrameSize.X, (int)FrameSize.Y);
                case TeddySpriteState.Blink4:
                    return new Rectangle(150, 75, (int)FrameSize.X, (int)FrameSize.Y);
                default:
                    return null;
            }

        }

        public virtual GeometryMethods.RectangleF TeddyRectangle
        {
            get 
            {
                if (_currentPowerup != null)
                    return _currentPowerup.GetExpandedPowerupRectangle(this);
                else
                    return JustTeddyRectangle;
            }
        }

        public GeometryMethods.RectangleF JustTeddyRectangle
        {
            get {
                return new GeometryMethods.RectangleF(Position.X, Position.Y,  this.FrameSize.X, this.FrameSize.Y); 
            }

        }

        #endregion

        public void MoveLeft(int speed)
        {
            if (_facing == Direction.Left)
            {
                if (!_isJumping)
                    _facingCounter++;

                if (speed == runSpeed)
                {
                    if (!_isJumping)
                        _facingCounter++;
                }
            }
            else
            {
                Facing = Direction.Left;
                _facingCounter = 0;
            }

            _playerOverallVelocity.X = -speed;
        }

        public void MoveRight(int speed)
        {
            if (_facing == Direction.Right)
            {
                if (!_isJumping)
                    _facingCounter++;

                if (speed == runSpeed)
                {
                    if (!_isJumping)
                        _facingCounter++;
                }
            }
            else
            {
                Facing = Direction.Right;
                 _facingCounter = 0;
            }

            _playerOverallVelocity.X = speed;
        }

        public Vector2 Velocity
        {
            get
            {
                return new Vector2( _playerOverallVelocity.X, _yVelocity);
            }
        }

        public void SetVelocity(Vector2 v)
        {
            _playerOverallVelocity.X = v.X;
            _yVelocity = v.Y;
        }

        public void SetTerminalVelocity( int v)
        {
            TERMINAL_VELOCITY = v;
        }

        public TeddySpriteState SpriteState { get { return _currentSprite; } }

        public void SetFacingSprite()
        {
            if (_facingCounter < _poseLengthWalk)
            {
                _currentSprite = TeddySpriteState.Run1;
            }
            else if (_facingCounter < _poseLengthWalk * 2)
            {
                _currentSprite = TeddySpriteState.Run2;
            }
            else if (_facingCounter < _poseLengthWalk * 3)
            {
                _currentSprite = TeddySpriteState.Run3;
            }
            else
            {
                _facingCounter = 0;
            }
        }

        public void SetPowerup(Powerup p)
        {
            _currentPowerup = p;
        }

        public override void Update(GameTime gameTime)
        {
            Screen currentScreen = (Screen)Game.Components[0];
            KeyboardState keyState = Keyboard.GetState();

            if (_currentPowerup != null)
                if (!_currentPowerup.Update(gameTime, currentScreen,this, keyState)) { return; }



            _playerOverallVelocity = new Vector2(0,0);
            if (_ridingSurface != null)
            {
                _playerOverallVelocity = _ridingSurface.SurfaceVelocity();
            }
            

            int speed = walkSpeed;

            if (keyState.IsKeyDown(Keys.LeftShift))
            {
                speed = runSpeed;
                _isRunning = true;
            }
            else
            {
                _isRunning = false;
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                MoveLeft( speed);
                SetFacingSprite();
            }


            if (keyState.IsKeyDown(Keys.Right))
            {
                MoveRight(speed);
                SetFacingSprite();
            }



            //if both right and left keys are up, teddy should be facing forward (he'll blink)
            if (keyState.IsKeyUp(Keys.Right) && keyState.IsKeyUp(Keys.Left))
            {
                if (_facing == Direction.Up)
                {
                    _facingCounter++;
                }
                else
                {
                    Facing = Direction.Up;
                    _facingCounter = 0;
                }

                if (_facingCounter < _initialBlinkWaitCounter )
                {
                    _currentSprite = TeddySpriteState.Blink1;
                    if (_facingCounter == _initialBlinkWaitCounter - 1)
                    {
                        _initialBlinkWaitCounter = 0;
                        _facingCounter = 1;
                    }
                }
                else if (_facingCounter < _blinkPoseLength * 2)
                {
                    _currentSprite = TeddySpriteState.Blink1;
                }
                else if (_facingCounter < _blinkPoseLength * 3)
                {
                    _currentSprite = TeddySpriteState.Blink2;
                }
                else if (_facingCounter < _blinkPoseLength * 4)
                {
                    _currentSprite = TeddySpriteState.Blink3;
                }
                else if (_facingCounter < _blinkPoseLength * 5)
                {
                    _currentSprite = TeddySpriteState.Blink4;
                }
                else if (_facingCounter < _blinkPoseLength * 6)
                {
                    _currentSprite = TeddySpriteState.Blink3;
                }
                else if (_facingCounter < _blinkPoseLength * 7)
                {
                    _currentSprite = TeddySpriteState.Blink2;
                }
                else if (_facingCounter < _blinkPoseLength * 8)
                {
                    _currentSprite = TeddySpriteState.Blink1;
                }
                else if (_facingCounter < _blinkPoseLength * 9)
                {
                    _currentSprite = TeddySpriteState.Blink1;
                }
                else if (_facingCounter > _blinkPoseLength * 25)
                {
                    _facingCounter = 0;
                    _initialBlinkWaitCounter = _initialBlinkWait;
                }

            }


            if (Dead || LevelComplete)
            {
                if (Position.Y < currentScreen.CameraBounds.Height)
                {
                    Position = new Vector2(Position.X, Position.Y + 3);
                }

            }
            else
            {

                if (_isHit)
                {
                    if (_recoverCounter < _recoverWait)
                    {
                        _recoverCounter++;
                    }
                    else
                    {
                        _isHit = false;
                        _recoverCounter = 0;
                    }
                }

                bool canJump = true;
                if (_currentPowerup != null)
                    canJump = !_currentPowerup.PreventsJump(this);

                //if teddy is not jumping, and he is not falling, and the user hits the space bar, jump
                if (canJump && keyState.IsKeyDown(Keys.Space) && _isJumping == false && _yVelocity == 0.0f)
                {
                    _isJumping = true;
                    _yVelocity = -_initialJumpVelocity;
                    _spacePressedDown = true;
                    _ridingSurface = null;
                }

                if (keyState.IsKeyUp(Keys.Space))
                {
                    _spacePressedDown = false;
                }

                movePlayerAndCheckState(currentScreen, keyState);

            }

            //If teddy is dead, we still want to update powerups 
            if (_currentPowerup != null)
                _currentPowerup.AfterUpdate(gameTime, currentScreen, this, keyState);

        }

        public void movePlayerAndCheckState(Screen currentScreen, KeyboardState keyState)
        {

            movePlayerX(_playerOverallVelocity, currentScreen);


            checkForObjectInteractions(currentScreen);

            //Apply change in Y
            _yVelocity += _gravity;

            if (_yVelocity > TERMINAL_VELOCITY)
                _yVelocity = TERMINAL_VELOCITY;

            movePlayerY(keyState, ref _yVelocity, currentScreen);

            //Check if teddy has fallen down the well
            if (Position.Y > currentScreen.LevelHeight)
            {
                                       this.Dead = true;
            }

            if (_yVelocity > 30)
            {
                this.Dead = true;
            }

            //check to see if teddy is interacting with any enemies
            checkEnemies(keyState, currentScreen);
        }

        public void movePlayerX(Vector2 overallVelocity, Screen currentScreen)
        {

            if (overallVelocity.X != 0)
            {

                this.Position = new Vector2(this.Position.X + overallVelocity.X, this.Position.Y);

 
                if (this.Position.X < 0)
                {
                    this.Position = new Vector2(0, this.Position.Y);
                }
                else if ( this.Position.X + this.TeddyRectangle.Width > currentScreen.LevelWidth)
                {
                    this.Position = new Vector2(currentScreen.LevelWidth - this.TeddyRectangle.Width, this.Position.Y);
                }

                if (overallVelocity.X > 0)
                {
                    foreach (Surface surface in currentScreen.Surfaces)
                    {
                        if (this.TeddyRectangle.Intersects(surface.Rect))
                        {
                            if (this.TeddyRectangle.Right > surface.Left && this.TeddyRectangle.Left < surface.Left)
                            {
                                Position = new Vector2(surface.Left - (int)this.TeddyRectangle.Width, Position.Y);
                                break;
                            }


                        }
                    }
                }
                else if (overallVelocity.X < 0)
                {
                    foreach (Surface surface in currentScreen.Surfaces)
                    {
                        if (this.TeddyRectangle.Intersects(surface.Rect))
                        {
                            if (this.TeddyRectangle.Left > surface.Right && this.TeddyRectangle.Right < surface.Right)
                            {
                                Position = new Vector2(surface.Right, Position.Y);
                                break;
                            }


                        }
                    }
                }


            }

        }
        
        protected void movePlayerY( KeyboardState keyState, ref float yVelocity, Screen currentScreen)
        {

            _position = new Vector2(_position.X, _position.Y + yVelocity);

            if (_ridingSurface != null)
            {
                //Check if teddy is still riding on this surface
                if (this.TeddyRectangle.Intersects(_ridingSurface.SurfaceBounds()) && (this.TeddyRectangle.Bottom >= _ridingSurface.SurfaceBounds().Top))
                {
                    //Still Riding on surface
                    _position.Y = _ridingSurface.SurfaceBounds().Top - this.TeddyRectangle.Height;
                    yVelocity = 0.0f;
                    _isJumping = false;
                }else {
                    if (!_ridingSurface.SurfaceOwner().PlayerIsSteering)
                    {
                        //Either Teddy fell off, or jumped off.  Either way, we are no longer riding.
                        _ridingSurface = null;
                    }

                }
            }


            foreach (Surface surfaceRect in currentScreen.Surfaces)
            {
                //Negative velocity is up (so teddy is jumping)
                if (yVelocity < 0)
                {
                    //Check for surfaces above (Teddy hit his head)
                    if (this.TeddyRectangle.Intersects(surfaceRect.Rect) & (this.TeddyRectangle.Top < surfaceRect.Bottom && this.TeddyRectangle.Bottom > surfaceRect.Bottom))
                    {
                        //todo: if teddy is really close to one of the edge (only 1 pixel is touching the above surface, then move him over.  I'm pretty sure  Mario does that.

                        this.Position = new Vector2(Position.X, surfaceRect.Bottom + 1);
                        yVelocity = 0.0f;
                    }
                }
                else
                {
                    //Check for surfaces below
                    if (this.TeddyRectangle.Intersects(surfaceRect.Rect) & (this.TeddyRectangle.Bottom > surfaceRect.Top))
                    {
                        Position = new Vector2(Position.X, surfaceRect.Top - this.TeddyRectangle.Height);
                        yVelocity = 0.0f;
                        _isJumping = false;
                    }
                }


            }
        }

        protected virtual void checkForObjectInteractions(Screen currentScreen)
        {
            foreach (GameObject f in currentScreen.GameObjects)
            {
                if (f.GetType() == typeof(Goal))
                {
                    if (this.TeddyRectangle.Intersects(f.CollisionRectangle))
                    {
                        Position = new Vector2(-100, -100);
                        this.LevelComplete = true;
                    }
                }
                else if (f.GetType() == typeof(Fluff))
                {
                    if (!f.Destroyed & this.TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                    {
                        HandleFluffGrab((Fluff)f);
                    }
                }
                else if (f.GetType() == typeof(Goggles))
                {
                    if (!f.Destroyed & this.TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                    {
                        f.Destroyed = true;
                        _currentPowerup = new GogglesPowerup(_game);
                    }

                }
                else if (f.GetType() == typeof(PulseArmPickup))
                {
                    if (!f.Destroyed & this.TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                    {
                        f.Destroyed = true;
                        _currentPowerup = new PulseArmPowerup(_game);
                    }

                }
                else if (f.GetType() == typeof(BlanketPickup))
                {
                    if (!f.Destroyed && this.TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                    {
                        f.Destroyed = true;
                        _currentPowerup = new Powerups.BlanketPowerup(_game);
                    }
                }
                else if (f.GetType() == typeof(NightVision))
                {
                    if (!f.Destroyed && this.TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                    {
                        f.Destroyed = true;
                        _currentPowerup = new NightVisionPowerup(_game);
                    }
                }

            }
        }

        protected virtual void HandleFluffGrab(Fluff f)
        {
            bool continueGrab = true;
            
            if (_currentPowerup != null)
                continueGrab = _currentPowerup.HandleFluffGrab(this,  f);

            if (continueGrab){
                _currentFluff++;
                f.Destroyed = true;
            }

        }


        protected void checkEnemies(KeyboardState keyState, Screen currentScreen)
        {

            foreach (Enemy e in currentScreen.Enemies)
            {
                checkLandingOnEnemy(e, keyState);

                checkTeddyEnemyInteraction(e, currentScreen, keyState);
                
                if (e.ChildrenEnemies != null)
                {
                    foreach (Enemy e2 in e.ChildrenEnemies)
                    {
                        checkLandingOnEnemy(e2, keyState);
                        checkTeddyEnemyInteraction(e2, currentScreen, keyState);
                    }
                }

            }
        }

        private void checkLandingOnEnemy(Enemy e, KeyboardState keyState)
        {
            
            if (_ridingSurface == null & ((e.CanJumpOnToKill || e.PlayerCanRide) && e.CanInteractWithPlayer))
            {
                 GeometryMethods.RectangleF rHit = null;
                if ((e.RectangleInsersectsWithHitBoxes(this.TeddyRectangle, ref rHit)) & (this.TeddyRectangle.Bottom - _yVelocity <= e.CollisionRectangle.Top))
                {
                    HandleLandingOnEnemy(e, keyState);
                }
            }
        }

        private void checkTeddyEnemyInteraction(Enemy e, Screen currentScreen, KeyboardState keyState)
        {
            GeometryMethods.RectangleF rHit = null;

            if (e.CanInteractWithPlayer & (e.RectangleInsersectsWithHitBoxes(this.TeddyRectangle, ref rHit)) && (e != _ridingSurface))
            {
                HandleEnemyInteraction( e, currentScreen, rHit);
            }

        }

        protected virtual void HandleLandingOnEnemy(Enemy e, KeyboardState keyState)
        {

            if (_currentPowerup != null)
                if (!_currentPowerup.HandleLandingOnEnemy(e, keyState)) { return;}

            if (e.CanJumpOnToKill)
            {
                _enemiesDestroyed++;
                e.DoDamage(_damage);
                if (keyState.IsKeyDown(Keys.Space))
                {
                    //Can get extra boost jumping off of enemy
                    _yVelocity = -_initialJumpVelocity - 3;
                    _isJumping = true;
                }
                else
                {
                    //If we are not jumping then just give teddy a little boost
                    _yVelocity = -3;
                }

            }
            else if (e.PlayerCanRide || e.PlayerIsSteering)
            {
                _ridingSurface = (ISurfaceInterface)e;
                _position.Y = _ridingSurface.SurfaceBounds().Top - this.TeddyRectangle.Height - 1;
            }
        }

        protected virtual void HandleEnemyInteraction(Enemy e, Screen currentScreen, GeometryMethods.RectangleF enemyHitBox)
        {

            if (_currentPowerup != null)
                if (!_currentPowerup.HandleEnemyInteraction( this,e, currentScreen, enemyHitBox)) { return; }

            _currentSprite = TeddySpriteState.Blink4;
            // Check if Teddy has been hit
            if (!_isHit && e.BeingDamaged == false)
            {
                if (e.Damage > CurrentFluff)
                {
                    Dead = true;
                    e.DoDamage(_damage);
                }
                else
                {
                    
                    _isHit = true;

                    var playerOverallVelocity = new Vector2(-50, 0);
                    //hit on right side
                    if (this.TeddyRectangle.Left <= enemyHitBox.Left)
                    {
                        _yVelocity = -3;
                        playerOverallVelocity.X = -50;
                    }
                    else //hit on left side
                    {
                        _yVelocity = -3;
                        playerOverallVelocity.X = 50;
                    }

                    movePlayerX(playerOverallVelocity, currentScreen);
                    if (!WearingPowerup)
                    {
                        CurrentFluff -= e.Damage;
                        throwFluff(e.Damage);

                    } else {

                        if (_currentPowerup.AfterTeddyDamage(this))
                        {
                            _currentPowerup = null;
                        } else{
                            CurrentFluff -= e.Damage;
                            throwFluff(e.Damage);
                        }

                    }

                    
                }
            }
        }
 

	    private void throwFluff(int damage)
        {
            double mid = TeddyRectangle.Height / 2;
            Random r = new Random();
            Screen currentScreen = (Screen)this.Game.Components[0];

            for (int i = 0; i < damage; i++)
            {
                Vector2 fluffPos = new Vector2(TeddyRectangle.Left, TeddyRectangle.Top);
                fluffPos.X -= r.Next(-100, 100);
                fluffPos.Y -= r.Next(50, 100);
                float xVelocity = (float)r.NextDouble() * r.Next(-1, 2) * 5f + 0.5f;
                float yVelocity = (float)r.NextDouble() * -10f;
                Fluff thrown = new Fluff(this.Game, fluffPos, true, xVelocity, yVelocity, true);
                currentScreen.GameObjects.Add(thrown);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch teddyBatch)
        {

            if (_currentPowerup != null)
            {
                if (!_currentPowerup.BeforeDraw(gameTime, teddyBatch, this)) { return; }
            }

            Rectangle? boxToDraw = GetBoxToDraw();

            if (boxToDraw != null)
            {

                SpriteEffects seff = SpriteEffects.None;
                switch (_facing)
                {
                    case Direction.Left:
                        seff = SpriteEffects.FlipHorizontally;
                        break;
                    case Direction.Down:
                        seff = SpriteEffects.FlipVertically;
                        break;
                }

                if (_isRunning && _isJumping == false && _yVelocity == 0)
                {
                    teddyBatch.Draw(_runGlow, new Vector2(Position.X, Position.Y + boxToDraw.Value.Height - _runGlow.Height / 2), new Rectangle(0, 0, _runGlow.Width, _runGlow.Height), Color.White);
                }

                teddyBatch.Draw(this.StyleSheet, this.Position, boxToDraw, Color.White, 0, Vector2.Zero, 1, seff, 0);

                if (_currentPowerup != null)
                {
                    _currentPowerup.AfterDraw(gameTime, teddyBatch, this, seff);
                }


            }
        }

    }
}
