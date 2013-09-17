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
        private Texture2D _runGlow;
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
        private bool _isRunning = false;

        private bool _isJumping = false;
        private float _initialJumpVelocity = 10;
        private float _gravity = .25f;
        private float _yVelocity = 0.0f;
        
        private int _currentFluff = 100;
        private int _enemiesDestroyed = 0;
        private ISurfaceInterface _ridingSurface = null;

        private int _recoverCounter = 0;
        private int _recoverWait = 100;
        private bool _isHit = false;

        private bool _levelComplete = false;

        public void Initialize()
        {
            
        }

        public Teddy(Game game, Texture2D styleSheet, Vector2 initialPosition, Vector2 sizeOfFrame)
            :base(game)
        {
            StyleSheet = styleSheet;
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

        private GeometryMethods.RectangleF TeddyRectangle
        {
            get 
            {
                return new GeometryMethods.RectangleF(Position.X, Position.Y,  BoxToDraw.Width, BoxToDraw.Height); 
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (Dead || LevelComplete)
            {
                if (Position.Y < Game.GraphicsDevice.Viewport.Width)
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

                playerOverallVelocity.X = speed;

            }

            movePlayerX(playerOverallVelocity);

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
            checkForGoal();

            movePlayerY(keyState);
            
            //check to see if teddy is dead.
            checkForDeath(keyState);
        }

        private void movePlayerX(Vector2 overallVelocity)
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

                        foreach (Surface surface in ((Screen)Game.Components[0]).Surfaces)
                        {
                            if (TeddyRectangle.Intersects(surface.Rect) )
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

                    foreach (Surface surface in ((Screen)Game.Components[0]).Surfaces)
                    {
                        if (TeddyRectangle.Intersects(surface.Rect))
                        {
                            ((Screen)Game.Components[0]).MoveX(-(surface.Right - (int)this.Position.X));
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

                        foreach (Surface surface in ((Screen)Game.Components[0]).Surfaces)
                        {
                            if (TeddyRectangle.Intersects(surface.Rect))
                            {
                                Position = new Vector2(surface.Left - (int)TeddyRectangle.Width  , Position.Y);
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

                    foreach (Surface surface in ((Screen)Game.Components[0]).Surfaces)
                    {
                        if (TeddyRectangle.Intersects(surface.Rect))
                        {
                            ((Screen)Game.Components[0]).MoveX(-(surface.Left - (int)this.Position.X - (int)TeddyRectangle.Width));
                            break;
                        }
                    }
                }

            }
        }
        
        private void movePlayerY( KeyboardState keyState)
        {
            _yVelocity += _gravity;

            Position = new Vector2(Position.X, Position.Y + _yVelocity);

            if (_ridingSurface != null)
            {
                //Check if teddy is still riding on this surface
                if (TeddyRectangle.Intersects(_ridingSurface.SurfaceBounds()) && (TeddyRectangle.Bottom >= _ridingSurface.SurfaceBounds().Top))
                {
                    //Still Riding on surface
                    _position.Y = _ridingSurface.SurfaceBounds().Top - TeddyRectangle.Height;
                    _yVelocity = 0.0f;
                    _isJumping = false;
                }else {
                    //Either Teddy fell off, or jumped off.  Either way, we are no longer riding.
                    _ridingSurface = null;
                }
            }


            foreach (Surface surfaceRect in ((Screen)Game.Components[0]).Surfaces)
            {
                //Negative velocity is up (so teddy is jumping)
                if (_yVelocity < 0)
                {
                    //Check for surfaces above (Teddy hit his head)
                    if (TeddyRectangle.Intersects(surfaceRect.Rect) & (TeddyRectangle.Top < surfaceRect.Bottom))
                    {
                        //todo: if teddy is really close to one of the edge (only 1 pixel is touching the above surface, then move him over.  I'm pretty sure  Mario does that.

                          Position = new Vector2(Position.X, surfaceRect.Bottom + 1 );
                        _yVelocity = 0.0f;
                    }
                }
                else
                {
                    //Check for surfaces below
                    if (TeddyRectangle.Intersects(surfaceRect.Rect) & (TeddyRectangle.Bottom > surfaceRect.Top))
                    {
                        Position = new Vector2(Position.X, surfaceRect.Top - BoxToDraw.Height );
                        _yVelocity = 0.0f;
                        _isJumping = false;
                    }
                }


            }
        }

        private void checkForFluffGrabs()
        {
            foreach (GameObject f in ((Screen)Game.Components[0]).GameObjects)
            {
                if (f.GetType() == typeof(Fluff))
                {
                    if (!f.Destroyed & TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                    {
                        _currentFluff++;
                        f.Destroyed = true;
                    }
                }

            }
        }

        private void checkForGoal()
        {
            foreach (GameObject f in ((Screen)Game.Components[0]).GameObjects)
            {
                if (f.GetType() == typeof(Goal))
                {
                    if (TeddyRectangle.Intersects(f.CollisionRectangle))
                    {
                        Position = new Vector2(-100, -100);
                        this.LevelComplete = true;
                    }  
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

            GeometryMethods.RectangleF rHit = null;

            if (e.CanInteractWithPlayer & (e.RectangleInsersectsWithHitBoxes(TeddyRectangle, ref rHit)) && (e != _ridingSurface))
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
                        if (TeddyRectangle.Left <= rHit.Left)
                        {
                            _yVelocity = -3;
                            playerOverallVelocity.X = -50;
                            movePlayerX(playerOverallVelocity);
			                throwFluff(e.Damage);
                        }
                        else //hit on left side
                        {
                            _yVelocity = -3;
                            playerOverallVelocity.X = 50;
                            movePlayerX(playerOverallVelocity);
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

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            if (_isRunning && _isJumping == false && _yVelocity == 0)
            {
                spriteBatch.Draw(_runGlow, new Vector2(Position.X, Position.Y + BoxToDraw.Height - _runGlow.Height/2), new Rectangle(0,0, _runGlow.Width, _runGlow.Height), Color.White);
            }

            spriteBatch.Draw(StyleSheet, Position, BoxToDraw, Color.White);
            spriteBatch.End();
        }

    }
}
