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
        private int _jumpSpeed = 5;
        private int _jumpCounter = 0;
        private bool _jumpIsFalling = false;
        private int _totalJumpHeight = 350;
        private int _gravitySpeed = 3;

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

            int pixelsOver = 0;

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

                //if the global position of the screen is greater than or equal to zero we are the very beginning of the level, so move teddy, not the screen behind him.
                //OR if we are at the very end of the level, move teddy and not the level behind him
                if (((Screen)Game.Components[0]).GlobalPosition.X >= 0 || ((Position.X >= Game.GraphicsDevice.Viewport.Width / 2) && ((Screen)Game.Components[0]).TeddyAtLastScreen(ref pixelsOver)))
                {
                    if (Position.X > 0)
                    {
                        Position = new Vector2(Position.X - speed, Position.Y);

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
                    ((Screen)Game.Components[0]).MoveX(speed);

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


                //If we are at a position less than half of the view, move teddy 
                //OR if we are at the very last screen of the level, move teddy
                //OTHERWISE we move the screen behind teddy
                if ((Position.X <= Game.GraphicsDevice.Viewport.Width / 2) || ((Screen)Game.Components[0]).TeddyAtLastScreen(ref pixelsOver))
                {
                    if (pixelsOver != 0)
                    {
                        ((Screen)Game.Components[0]).MoveX(-pixelsOver);

                        Position = new Vector2(Position.X + speed, Position.Y);

                        if (Position.X + BoxToDraw.Width >= Game.GraphicsDevice.Viewport.Width)
                        {
                            Position = new Vector2(Game.GraphicsDevice.Viewport.Width - BoxToDraw.Width, Position.Y);
                        }
                    }
                    else
                    {
                        Position = new Vector2(Position.X + speed, Position.Y);

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
                    ((Screen)Game.Components[0]).MoveX(-speed);

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

            //if teddy is not jumping, and he is not falling, and the user hits the space bar, jump
            if (_isJumping == false & _jumpIsFalling == false & keyState.IsKeyDown(Keys.Space))
            {
                _isJumping = true;
            }

            //perform jump update logic
            if (_isJumping == true)
            {
                if (_jumpIsFalling == false)
                {
                    Position = new Vector2(Position.X, Position.Y - _jumpSpeed);

                    foreach (Rectangle surface in ((Screen)Game.Components[0]).Surfaces)
                    {
                        if (surface.Intersects(TeddyRectangle))
                        {
                            Position = new Vector2(Position.X, surface.Bottom + 1);
                            _jumpCounter = 0;
                            _isJumping = false;
                            _jumpIsFalling = true;
                            break;
                        }
                    }
                }
             
                _jumpCounter++;

                if ( _jumpCounter > _totalJumpHeight / _jumpSpeed)
                {
                    _jumpCounter = 0;
                    _isJumping = false;
                    _jumpIsFalling = true;
                }

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

            //if we are not currently jumping, apply gravity logic
            if (_isJumping == false)
            {
                applyGravity();
            }
            
            //check to see if teddy is dead.
            checkForDeath();
        }

        private void applyGravity()
        {
            Position = new Vector2(Position.X, Position.Y + _gravitySpeed);
            _jumpIsFalling = true;

            foreach (Rectangle surfaceRect in ((Screen)Game.Components[0]).Surfaces)
            {
                if (TeddyRectangle.Intersects(surfaceRect) & (TeddyRectangle.Bottom > surfaceRect.Top))
                {
                    Position = new Vector2(Position.X, surfaceRect.Top - BoxToDraw.Height);
                    _jumpIsFalling = false;
                }
            }
        }

        private void checkForFluffGrabs()
        {
            foreach (Fluff f in ((Screen)Game.Components[0]).Fluffs)
            {
                if (!f.Destroyed & TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                {
                    f.Destroyed = true;
                }
            }
        }

        private void checkForDeath()
        {
            if (Position.Y > Game.GraphicsDevice.Viewport.Height)
            {
                Dead = true;
            }

            foreach (Enemy e in ((Screen)Game.Components[0]).Enemies)
            {
                if (e.CanJumpOnToKill && e.Destroyed == false)
                {
                    if ((TeddyRectangle.Intersects(e.CollisionRectangle)) & (TeddyRectangle.Bottom - _gravitySpeed <= e.CollisionRectangle.Top))
                    {
                        e.Destroyed = true;
                    }
                }

                if (e.Destroyed == false & (TeddyRectangle.Intersects(e.CollisionRectangle)))
                {
                    BoxToDraw = new Rectangle(150, 75, BoxToDraw.Width, BoxToDraw.Height);
                    Dead = true;
                    break;
                }

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
