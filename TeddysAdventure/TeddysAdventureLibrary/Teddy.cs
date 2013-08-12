using System;
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

        private int _x;
        private int _y;
        private int _height;
        private int _width;
        private Texture2D _styleSheet;
        private Game _game;
        private Vector2 _position;
        private Rectangle _collisionBox;
        private Rectangle _boxToDraw;
        private Vector2 _frameSize;
        private int walkSpeed = 2;
        private int runSpeed = 3;
        private Direction _facing = Direction.Up;
        private int _facingCounter = 0;
        private int _poseLengthWalk = 15;
        private int _initialBlinkWaitCounter = 500;
        private int _initialBlinkWait = 500;
        private int _blinkPoseLength = 10;
        private bool _dead = false;

        private bool _isJumping = false;
        private float _initialJumpVelocity = 10;
        private float _gravity = .25f;
        private float _yVelocity = 0.0f;
        
        private int _currentFluff = 0;
        private int _enemiesDestroyed = 0;
        private ISurfaceInterface _ridingSurface = null;


        public void Initialize()
        {
            
        }

        public Teddy(Game game, Texture2D styleSheet, Vector2 initialPosition, Vector2 sizeOfFrame)
            :base(game)
        {
            StyleSheet = styleSheet;
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

        public Rectangle TeddyRectangle
        {
            get 
            {
                return new Rectangle((int)Position.X, (int)Position.Y, BoxToDraw.Width, BoxToDraw.Height); 
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (Dead)
            {
                if (Position.Y < Game.GraphicsDevice.Viewport.Width)
                {
                    Position = new Vector2(Position.X, Position.Y + 3);
                }
                return;
            }


            var playerOverallVelocity = new Vector2(0,0);
            if (_ridingSurface != null)
            {
                playerOverallVelocity = _ridingSurface.SurfaceVelocity();
            }

            KeyboardState keyState = Keyboard.GetState();
            int speed = walkSpeed;

            if (keyState.IsKeyDown(Keys.LeftShift))
            {
                speed = runSpeed;
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                if (_facing == Direction.Left)
                {
                    _facingCounter++;
                    if (speed == runSpeed)
                    {
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
                    BoxToDraw = new Rectangle(100, 0, BoxToDraw.Width, BoxToDraw.Height);
                }
                else
                {
                    BoxToDraw = new Rectangle(150, 0, BoxToDraw.Width, BoxToDraw.Height);

                    if (_facingCounter == _poseLengthWalk * 2)
                    {
                        _facingCounter = 0;
                    }
                }

                if (((Screen)Game.Components[0]).GlobalPosition.X > 0)
                {
                    ((Screen)Game.Components[0]).GlobalPosition = new Vector2(0, 0);
                }

                playerOverallVelocity.X = -speed;
            }


            if (keyState.IsKeyDown(Keys.Right))
            {
                if (_facing == Direction.Right)
                {
                    _facingCounter++;
                    
                    if (speed == runSpeed)
                    {
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
                else
                {
                    BoxToDraw = new Rectangle(50, 0, BoxToDraw.Width, BoxToDraw.Height);

                    if (_facingCounter == _poseLengthWalk * 2)
                    {
                        _facingCounter = 0;
                    }
                }

                playerOverallVelocity.X = speed;

            }

            movePlayer(playerOverallVelocity);

            //if teddy is not jumping, and he is not falling, and the user hits the space bar, jump
            if ( keyState.IsKeyDown(Keys.Space) && _isJumping == false && _yVelocity == 0.0f)
            {
                _isJumping = true;
                _yVelocity = -_initialJumpVelocity;
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

            checkForFluffGrabs();

            applyGravity(keyState);
            
            //check to see if teddy is dead.
            checkForDeath(keyState);
        }

        private void movePlayer(Vector2 overallVelocity)
        {


            int pixelsOver = 0;

            if (overallVelocity.X < 0) //Moving Left
            {

                //if the global position of the screen is greater than or equal to zero we are the very beginning of the level, so move teddy, not the screen behind him.
                //OR if we are at the very end of the level, move teddy and not the level behind him
                if (((Screen)Game.Components[0]).GlobalPosition.X >= 0 || ((Position.X >= Game.GraphicsDevice.Viewport.Width / 2) && ((Screen)Game.Components[0]).TeddyAtLastScreen(ref pixelsOver)))
                {
                    //Move Teddy
                    if (Position.X > 0)
                    {
                        Position = new Vector2(Position.X + overallVelocity.X, Position.Y);

                        foreach (Rectangle surface in ((Screen)Game.Components[0]).Surfaces)
                        {
                            if (surface.Intersects(TeddyRectangle))
                            {
                                Position = new Vector2(surface.Right + 1, Position.Y);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //Move Screen
                    ((Screen)Game.Components[0]).MoveX((int)-overallVelocity.X);

                    foreach (Rectangle surface in ((Screen)Game.Components[0]).Surfaces)
                    {
                        if (surface.Intersects(TeddyRectangle))
                        {
                            ((Screen)Game.Components[0]).MoveX(-(surface.Right - TeddyRectangle.X));
                            break;
                        }
                    }
                }


            }
            else if (overallVelocity.X > 0) //Moving Right
            {

                //If we are at a position less than half of the view, move teddy 
                //OR if we are at the very last screen of the level, move teddy
                //OTHERWISE we move the screen behind teddy
                if ((Position.X <= Game.GraphicsDevice.Viewport.Width / 2) || ((Screen)Game.Components[0]).TeddyAtLastScreen(ref pixelsOver))
                {
                    if (pixelsOver != 0)
                    {
                        ((Screen)Game.Components[0]).MoveX(-pixelsOver);

                        Position = new Vector2(Position.X + overallVelocity.X, Position.Y);

                        if (Position.X + BoxToDraw.Width >= Game.GraphicsDevice.Viewport.Width)
                        {
                            Position = new Vector2(Game.GraphicsDevice.Viewport.Width - BoxToDraw.Width, Position.Y);
                        }
                    }
                    else
                    {
                        Position = new Vector2(Position.X + overallVelocity.X, Position.Y);

                        foreach (Rectangle surface in ((Screen)Game.Components[0]).Surfaces)
                        {
                            if (surface.Intersects(TeddyRectangle))
                            {
                                Position = new Vector2(surface.Left - TeddyRectangle.Width - 1, Position.Y);
                                break;
                            }
                        }

                        if (Position.X + BoxToDraw.Width >= Game.GraphicsDevice.Viewport.Width)
                        {
                            Position = new Vector2(Game.GraphicsDevice.Viewport.Width - BoxToDraw.Width, Position.Y);
                        }
                    }

                }
                else
                {
                    ((Screen)Game.Components[0]).MoveX((int)-overallVelocity.X);

                    foreach (Rectangle surface in ((Screen)Game.Components[0]).Surfaces)
                    {
                        if (surface.Intersects(TeddyRectangle))
                        {
                            ((Screen)Game.Components[0]).MoveX(-(surface.Left - TeddyRectangle.X - TeddyRectangle.Width));
                            break;
                        }
                    }
                }

            }
        }


        private void applyGravity( KeyboardState keyState)
        {
            _yVelocity += _gravity;

            Position = new Vector2(Position.X, Position.Y + _yVelocity);

            if (_ridingSurface != null)
            {
                //Check if teddy is still riding on this surface
                if (TeddyRectangle.Intersects(_ridingSurface.SurfaceBounds()) && (TeddyRectangle.Bottom > _ridingSurface.SurfaceBounds().Top))
                {
                    //Still Riding on surface
                    _position.Y = _ridingSurface.SurfaceBounds().Top - TeddyRectangle.Height - 1;
                    _isJumping = false;
                }else {
                    //Either Teddy fell off, or jumped off.  Either way, we are no longer riding.
                    _ridingSurface = null;
                }

            }


            foreach (Rectangle surfaceRect in ((Screen)Game.Components[0]).Surfaces)
            {
                if (TeddyRectangle.Intersects(surfaceRect) & (TeddyRectangle.Bottom > surfaceRect.Top))
                {
                    Position = new Vector2(Position.X, surfaceRect.Top - BoxToDraw.Height);
                    _yVelocity = 0.0f;
                    _isJumping = false;
                }
            }
        }

        private void checkForFluffGrabs()
        {
            foreach (Fluff f in ((Screen)Game.Components[0]).Fluffs)
            {
                if (!f.Destroyed & TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                {
                    _currentFluff++;
                    f.Destroyed = true;
                }
            }
        }

        private void checkForDeath(KeyboardState keyState)
        {
            if (Position.Y > Game.GraphicsDevice.Viewport.Height)
            {
                Dead = true;
            }

            foreach (Enemy e in ((Screen)Game.Components[0]).Enemies)
            {
                checkEnemyInducedDeath(e, keyState);

                if (e.ChildrenEnemies != null)
                {
                    foreach (Enemy e2 in e.ChildrenEnemies)
                    {
                        checkEnemyInducedDeath(e2, keyState);
                    }
                }

            }
        }

        private void checkEnemyInducedDeath(Enemy e, KeyboardState keyState)
        {
            if ((e.CanJumpOnToKill || e.PlayerCanRide) && e.CanInteractWithPlayer)
            {
                if ((TeddyRectangle.Intersects(e.CollisionRectangle)) & (TeddyRectangle.Bottom - _yVelocity <= e.CollisionRectangle.Top))
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
                        _position.Y = _ridingSurface.SurfaceBounds().Top - TeddyRectangle.Height - 1;
                    }

                }
            }

            if (e.CanInteractWithPlayer & (TeddyRectangle.Intersects(e.CollisionRectangle)) && (e != _ridingSurface))
            {
                BoxToDraw = new Rectangle(150, 75, BoxToDraw.Width, BoxToDraw.Height);
                Dead = true;
                e.Kill();
            }

        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            spriteBatch.Draw(StyleSheet, Position, BoxToDraw, Color.White);
            spriteBatch.End();
        }

    }
}
