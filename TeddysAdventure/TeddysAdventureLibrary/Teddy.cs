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
        private int _poseLength = 15;
        private int _initialBlinkWaitCounter = 500;
        private int _initialBlinkWait = 500;
        private int _blinkPoseLength = 10;

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
        #endregion

        public override void Update(GameTime gameTime)
        {
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
                }
                else
                {
                    _facing = Direction.Left;
                    _facingCounter = 0;
                }

                if (_facingCounter < _poseLength)
                {
                    BoxToDraw = new Rectangle(100, 0, BoxToDraw.Width, BoxToDraw.Height);
                }
                else
                {
                    BoxToDraw = new Rectangle(150, 0, BoxToDraw.Width, BoxToDraw.Height);

                    if (_facingCounter == _poseLength * 2)
                    {
                        _facingCounter = 0;
                    }
                }

                if (((Screen)Game.Components[0]).Position.X == 0)
                {
                    if (Position.X > 0)
                    {
                        Position = new Vector2(Position.X - speed, Position.Y);
                    }
                }
                else
                {
                    ((Screen)Game.Components[0]).MoveX(speed);
                }
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                if (_facing == Direction.Right)
                {
                    _facingCounter++;
                }
                else
                {
                    _facing = Direction.Right;
                    _facingCounter = 0;
                }

                if (_facingCounter < _poseLength)
                {
                    BoxToDraw = new Rectangle(0, 0, BoxToDraw.Width, BoxToDraw.Height);
                }
                else
                {
                    BoxToDraw = new Rectangle(50, 0, BoxToDraw.Width, BoxToDraw.Height);

                    if (_facingCounter == _poseLength * 2)
                    {
                        _facingCounter = 0;
                    }
                }
                
                if (Position.X <= Game.GraphicsDevice.Viewport.Width / 2)
                {
                    Position = new Vector2(Position.X + speed, Position.Y);
                }
                else
                {
                    ((Screen)Game.Components[0]).MoveX(-speed);
                }
            }

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
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            spriteBatch.Draw(StyleSheet, Position, BoxToDraw, Color.White);
            spriteBatch.End();
        }

        private void processMovement(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Up))
            {
                
            }
        }

        private void move(Direction directionToMove, Screen currentScreen)
        {

        }


    }
}
