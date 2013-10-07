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

        protected int TERMINAL_VELOCITY = 10;

        private int _x;
        private int _y;
        private int _height;
        private int _width;
        private Texture2D _styleSheet;
        private Texture2D _runGlow;
        private Game _game;
        protected Vector2 _position;
        private Rectangle _collisionBox;
        private Rectangle _boxToDraw;
        private Vector2 _frameSize;
        protected int walkSpeed = 2;
        protected int runSpeed = 3;
        private Direction _facing = Direction.Up;
        private int _facingCounter = 0;
        private int _poseLengthWalk = 15;
        private int _initialBlinkWaitCounter = 500;
        private int _initialBlinkWait = 500;
        private int _blinkPoseLength = 10;
        private bool _dead = false;
        private bool _isRunning = false;

        private bool _isJumping = false;
        private float _initialJumpVelocity = 10;
        protected float _gravity = .25f;
        protected float _yVelocity = 0.0f;
        protected Vector2 _playerOverallVelocity;

        private int _currentFluff = 100;
        private int _enemiesDestroyed = 0;
        private ISurfaceInterface _ridingSurface = null;

        private int _recoverCounter = 0;
        private int _recoverWait = 100;
        private bool _isHit = false;
        

        private bool _levelComplete = false;

        //Keystate 
        protected bool _spacePressedDown = false;


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
            BoxToDraw = new Rectangle(200, 0, (int)sizeOfFrame.X, (int)sizeOfFrame.Y);

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        #region " Properties "
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

        public int EnemiesDestroyed
        {
            get { return _enemiesDestroyed; }
            set { _enemiesDestroyed = value; }
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

        public Rectangle BoxToDraw
        {
            get { return _boxToDraw; }
            set { _boxToDraw = value; }
        }

        public virtual GeometryMethods.RectangleF TeddyRectangle
        {
            get 
            {
                return new GeometryMethods.RectangleF(Position.X, Position.Y,  BoxToDraw.Width, BoxToDraw.Height); 
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {

            Screen currentScreen = (Screen)Game.Components[0];



            _playerOverallVelocity = new Vector2(0,0);
            if (_ridingSurface != null)
            {
                _playerOverallVelocity = _ridingSurface.SurfaceVelocity();
            }

            KeyboardState keyState = Keyboard.GetState();
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
                if (_facing == Direction.Left)
                {
                    if(!_isJumping)
                        _facingCounter++;

                    if (speed == runSpeed)
                    {
                        if (!_isJumping)
                            _facingCounter++;
                    }
                }
                else
                {
                    _facing = Direction.Left;
                    _facingCounter = 0;
                }

                if (_facingCounter < _poseLengthWalk)
                {
                    BoxToDraw = new Rectangle(150, 0, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _poseLengthWalk * 2)
                {
                    BoxToDraw = new Rectangle(200, 0, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _poseLengthWalk * 3)
                {
                    BoxToDraw = new Rectangle(250, 0, BoxToDraw.Width, BoxToDraw.Height);

                }
                else
                {
                    _facingCounter = 0;
                }

                _playerOverallVelocity.X = -speed;
            }


            if (keyState.IsKeyDown(Keys.Right))
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
                    _facing = Direction.Right;
                    _facingCounter = 0;
                }

                if (_facingCounter < _poseLengthWalk)
                {
                    BoxToDraw = new Rectangle(0, 0, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _poseLengthWalk * 2)
                {
                    BoxToDraw = new Rectangle(50, 0, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _poseLengthWalk * 3)
                {
                    BoxToDraw = new Rectangle(100, 0, BoxToDraw.Width, BoxToDraw.Height);
                }
                else
                {
                    _facingCounter = 0;
                }

                _playerOverallVelocity.X = speed;

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
                    _facing = Direction.Up;
                    _facingCounter = 0;
                }

                if (_facingCounter < _initialBlinkWaitCounter )
                {
                    BoxToDraw = new Rectangle(0, 75, BoxToDraw.Width, BoxToDraw.Height);
                    if (_facingCounter == _initialBlinkWaitCounter - 1)
                    {
                        _initialBlinkWaitCounter = 0;
                        _facingCounter = 1;
                    }
                }
                else if (_facingCounter < _blinkPoseLength * 2)
                {
                    BoxToDraw = new Rectangle(0, 75, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _blinkPoseLength * 3)
                {
                    BoxToDraw = new Rectangle(50, 75, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _blinkPoseLength * 4)
                {
                    BoxToDraw = new Rectangle(100, 75, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _blinkPoseLength * 5)
                {
                    BoxToDraw = new Rectangle(150, 75, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _blinkPoseLength * 6)
                {
                    BoxToDraw = new Rectangle(100, 75, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _blinkPoseLength * 7)
                {
                    BoxToDraw = new Rectangle(50, 75, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _blinkPoseLength * 8)
                {
                    BoxToDraw = new Rectangle(0, 75, BoxToDraw.Width, BoxToDraw.Height);
                }
                else if (_facingCounter < _blinkPoseLength * 9)
                {
                    BoxToDraw = new Rectangle(0, 75, BoxToDraw.Width, BoxToDraw.Height);
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
                return;
            }

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


            //if teddy is not jumping, and he is not falling, and the user hits the space bar, jump
            if (keyState.IsKeyDown(Keys.Space) && _isJumping == false && _yVelocity == 0.0f)
            {
                _isJumping = true;
                _yVelocity = -_initialJumpVelocity;
                _spacePressedDown = true;
            }

            if (keyState.IsKeyUp(Keys.Space))
            {
                _spacePressedDown = false;
            }

            movePlayerAndCheckState(currentScreen, keyState);

        }

        protected void movePlayerAndCheckState(Screen currentScreen, KeyboardState keyState)
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

        protected void movePlayerX(Vector2 overallVelocity, Screen currentScreen)
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
                            Position = new Vector2(surface.Left - (int)this.TeddyRectangle.Width, Position.Y);
                            break;
                        }
                    }
                }
                else if (overallVelocity.X < 0)
                {
                    foreach (Surface surface in currentScreen.Surfaces)
                    {
                        if (this.TeddyRectangle.Intersects(surface.Rect))
                        {
                            Position = new Vector2(surface.Right, Position.Y);
                            break;
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
                    //Either Teddy fell off, or jumped off.  Either way, we are no longer riding.
                    _ridingSurface = null;
                }
            }


            foreach (Surface surfaceRect in currentScreen.Surfaces)
            {
                //Negative velocity is up (so teddy is jumping)
                if (yVelocity < 0)
                {
                    //Check for surfaces above (Teddy hit his head)
                    if (this.TeddyRectangle.Intersects(surfaceRect.Rect) & (this.TeddyRectangle.Top < surfaceRect.Bottom))
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

        protected void checkForObjectInteractions(Screen currentScreen)
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
                        _currentFluff++;
                        f.Destroyed = true;
                    }
                }

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
            if ((e.CanJumpOnToKill || e.PlayerCanRide) && e.CanInteractWithPlayer)
            {
                if ((this.TeddyRectangle.Intersects(e.CollisionRectangle)) & (this.TeddyRectangle.Bottom - _yVelocity <= e.CollisionRectangle.Top))
                {
                    if (e.CanJumpOnToKill)
                    {
                        _enemiesDestroyed++;
                        e.Kill();
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
                    else if (e.PlayerCanRide)
                    {
                        _ridingSurface = (ISurfaceInterface)e;
                        _position.Y = _ridingSurface.SurfaceBounds().Top - this.TeddyRectangle.Height - 1;
                    }

                }
            }
        }

        private void checkTeddyEnemyInteraction(Enemy e, Screen currentScreen, KeyboardState keyState)
        {
            GeometryMethods.RectangleF rHit = null;

            if (e.CanInteractWithPlayer & (e.RectangleInsersectsWithHitBoxes(this.TeddyRectangle, ref rHit)) && (e != _ridingSurface))
            {
                BoxToDraw = new Rectangle(150, 75, BoxToDraw.Width, BoxToDraw.Height);
                // Check if Teddy has been hit
                if (!_isHit)
                {
                    if (e.Damage > CurrentFluff)
                    {
                        Dead = true;
                        e.Kill();
                    }
                    else
                    {
                        CurrentFluff -= e.Damage;
                        _isHit = true;
                        var playerOverallVelocity = new Vector2(-50, 0);
                        //hit on right side
                        if (this.TeddyRectangle.Left <= rHit.Left)
                        {
                            _yVelocity = -3;
                            playerOverallVelocity.X = -50;
                            movePlayerX(playerOverallVelocity, currentScreen);
			                throwFluff(e.Damage);
                        }
                        else //hit on left side
                        {
                            _yVelocity = -3;
                            playerOverallVelocity.X = 50;
                            movePlayerX(playerOverallVelocity, currentScreen);
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
                Fluff thrown = new Fluff(this.Game, fluffPos, true, xVelocity, yVelocity);
                currentScreen.GameObjects.Add(thrown);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch teddyBatch)
        {

            if (_isRunning && _isJumping == false && _yVelocity == 0)
            {
                teddyBatch.Draw(_runGlow, new Vector2(Position.X, Position.Y + BoxToDraw.Height - _runGlow.Height/2), new Rectangle(0,0, _runGlow.Width, _runGlow.Height), Color.White);
            }

            teddyBatch.Draw(StyleSheet, Position, BoxToDraw, Color.White);

        }

    }
}
