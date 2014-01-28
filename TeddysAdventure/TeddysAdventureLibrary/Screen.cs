using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TeddysAdventureLibrary
{
    public class Screen : DrawableGameComponent
    {
        public enum LevelType
        {
            Normal = 0,
            Jetpack = 1,
            UnderWater = 2,
            Falling = 3,
            Fluff = 4,
            Dark = 5
        }

        private Texture2D _deathSprite;
        private Texture2D _successSprite;
        private Texture2D _overlaySprite;
        private Dictionary<string,Texture2D> _surfaceTextures;
        private Dictionary<string, Texture2D> _backgroundImages;
        private SpriteFont _deathFont;
        private SpriteFont _hudFont;



        private List<Surface> _surfaces;
        private List<GameObject> _gameObjects;
        private List<Enemy> _enemies;
        private Teddy _teddy;



        private int _totalLevelWidth;
        private int _totalLevelHeight;
        private LevelType _levelType = LevelType.Normal;
        private long _milliSecondsElapsed = 0;
        private bool _timerStarted = false;

        private int _deathScreenCounter = 0;
        private bool _goBackToStartScreen = false;
        private bool _userPressedEnterToGoBack = false;

        private Color _backgroundColor;

        public static SpriteBatch _backgroundBatch;
        public static SpriteBatch _playerBatch;
        public static SpriteBatch _foregroundBatch;
        private static SpriteBatch _overlayBatch;

        public SpriteBatch OverlayBatch
        {
            get { return _overlayBatch; }
        }

        private Vector2 _currentCamera;
        private Rectangle _cameraBounds;
        private float _cameraZoom = 1.0f;

        public Rectangle CameraBounds { get { return _cameraBounds; } }

        public Teddy Teddy
        {
            get { return _teddy; }
            set { _teddy = value; }
        }

        public SpriteFont HudFont
        {
            get { return _hudFont; }
            set { _hudFont = value; }
        }

        public List<Surface> Surfaces
        {
            get { return _surfaces; }
            set { _surfaces = value; }
        }


        public List<GameObject> GameObjects
        {
            get { return _gameObjects; }
            set { _gameObjects = value; }
        }

        public List<Enemy> Enemies
        {
            get { return _enemies; }
            set { _enemies = value; }
        }

        public List<Background> Backgrounds { get; set; }

        public int LevelWidth { get { return _totalLevelWidth; } }
        public int LevelHeight { get { return _totalLevelHeight; } }

        public bool GoBackToStartScreen
        {
            get { return _goBackToStartScreen; }
            set { _goBackToStartScreen = value; }
        }

        public long MilliSecondsElapsed
        {
            get { return _milliSecondsElapsed; }
            set { _milliSecondsElapsed = value; }
        }

        public long SecondsElapsed
        {
            get { return _milliSecondsElapsed / 1000; }
            set { _milliSecondsElapsed = value * 1000; }
        }

        public Screen(Game game, string levelName)
            : base(game)
        {


            ScreenHelper screenHelper = game.Content.Load<ScreenHelper>("Screens\\" + levelName);

            _deathSprite = game.Content.Load<Texture2D>("Screens\\deathScreen");
            _successSprite = game.Content.Load<Texture2D>("Screens\\successScreen");
            _overlaySprite = game.Content.Load<Texture2D>("Screens\\tintOverlay");
            _deathFont = game.Content.Load<SpriteFont>("Fonts\\Arial16");
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\Arial12");

            _surfaceTextures = new Dictionary<string, Texture2D>();
            _backgroundImages = new Dictionary<string, Texture2D>();
            _surfaces = new List<Surface>();
            this.Backgrounds = new List<Background>();

            _backgroundColor = screenHelper.BackgroundColor;

            var allScreenSprites = from s in screenHelper.Surfaces select s.Sprite;

            foreach (BackgroundHelper bh in screenHelper.Backgrounds)
            {
                if (!_backgroundImages.ContainsKey(bh.Image))
                    _backgroundImages.Add(bh.Image, game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Screens\\Backgrounds", bh.Image)));

                this.Backgrounds.Add(new Background(bh));
            }

            foreach (SurfaceHelper sh in screenHelper.Surfaces)
            {
                if(!_surfaceTextures.ContainsKey(sh.Sprite) && sh.Sprite != string.Empty)
                    try {                    
                        _surfaceTextures.Add( sh.Sprite, game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", sh.Sprite))); 
                    }
                    catch (ContentLoadException cle)
                    {
                        Console.WriteLine("Could not load sprite " + sh.Sprite);
                    }
                _surfaces.Add(  new Surface(sh));

            }


            GameObjects = new List<GameObject>();
            foreach (GameObjectHelper v2 in screenHelper.ListOfObjects)
            {
                switch (v2.Type)
                {
                    case "Fluff":
                        GameObjects.Add(new Fluff(game, v2.Position));
                        break;
                    case "Goal":
                        GameObjects.Add(new Goal(game, v2.Position));
                        break;
                    case "BadApple":
                        GameObjects.Add(new BadApple(game, v2.Position));
                        break;
                    case "Goggles":
                        GameObjects.Add(new Goggles(game, v2.Position));
                        break;
                    case "PulseArmPickup":
                        GameObjects.Add(new PulseArmPickup(game, v2.Position));
                        break;
                    case "BlanketPickup":
                        GameObjects.Add(new BlanketPickup(game, v2.Position));
                        break;
                    case "NightVision":
                        GameObjects.Add(new NightVision(game, v2.Position));
                        break;
                }
                
            }

            _totalLevelWidth = (int)screenHelper.LevelSize.X;
            _totalLevelHeight = (int)screenHelper.LevelSize.Y;
            _enemies = new List<Enemy>();

            foreach (EnemyHelper eh in screenHelper.ListOfEnemies)
            {
                if (eh.IsSpawnPoint)
                {
                    _enemies.Add(new EnemySpawnPoint(game, eh.Position, eh.Type, eh.SpawnInterval, eh.Velocity));
                }
                else
                {
                    switch (eh.Type)
                    {
                        case "BowlingBall":
                            _enemies.Add(new BowlingBall(game, eh.Position, eh.Velocity));
                            break;
                        case "MatchBoxCar":
                            _enemies.Add(new MatchBoxCar(game, eh.Position, eh.Velocity));
                            break;
                        case "FlyingBook":
                            _enemies.Add(new FlyingBook(game, eh.Position, eh.Velocity));
                            break;
                        case "Airplane":
                            _enemies.Add(new PlaneEnemy(game, eh.Position, eh.Velocity));
                            break;
                        case "NightmareToaster":
                            _enemies.Add( new NightmareToaster(game, eh.Position, eh.Velocity));
                            break;
                        case "LadyBug":
                            _enemies.Add(new LadyBug(game, eh.Position, eh.Velocity));
                            break;
                        case "OrangeBomb":
                            _enemies.Add(new OrangeBomb(Game, eh.Position, eh.Velocity));
                            break;
                        case "OrangeCrayon":
                            _enemies.Add(new OrangeCrayon(Game, eh.Position, eh.Velocity));
                            break;
                        case "Eagle":
                            _enemies.Add(new Eagle(Game, eh.Position, eh.Velocity));
                            break;
                        case "DustBunny":
                            _enemies.Add(new DustBunny(Game, eh.Position, eh.Velocity));
                            break;
                        case "Vacuum":
                            _enemies.Add(new Vacuum(Game, eh.Position, eh.Velocity));
                            break;
                    }
                }
            }



            
            switch (screenHelper.LevelType)
            {
                case "Normal":
                    _levelType = LevelType.Normal;
                    break;
                case "Jetpack":
                    _levelType = LevelType.Jetpack;
                    break;
                case "Falling":
                    _levelType = LevelType.Falling;
                    break;
                case "UnderWater":
                    _levelType = LevelType.UnderWater;
                    break;
                case "Fluff":
                    _levelType = LevelType.Fluff;
                    break;
                case "Dark":
                    _levelType = LevelType.Dark;
                    break;
                default:
                    _levelType = LevelType.Normal;
                    break;
            }


            switch (_levelType)
            {
                case LevelType.Falling:
                    _teddy = new Teddy(game, screenHelper.TeddyStart, new Vector2(50,75));
                    break;
                case LevelType.Fluff:
                    _teddy = new Teddy(game, screenHelper.TeddyStart, new Vector2(50, 75));
                    _teddy.SetPowerup(new ZipperPowerup(game, _teddy, false));
                    break;
                default:
                    _teddy = new Teddy(game,    screenHelper.TeddyStart, new Vector2(50, 75));
                    break;
            }

            _playerBatch = new SpriteBatch(Game.GraphicsDevice);
            _foregroundBatch = new SpriteBatch(game.GraphicsDevice);
            _overlayBatch = new SpriteBatch(Game.GraphicsDevice);
            _backgroundBatch = new SpriteBatch(game.GraphicsDevice);
        }


        public override void Update(GameTime gameTime)
        {
            if (_timerStarted)
                _milliSecondsElapsed += gameTime.ElapsedGameTime.Milliseconds;

             KeyboardState keyState = Keyboard.GetState();

             if (keyState.IsKeyDown(Keys.Add)){
                 _cameraZoom += .01f;
             }else if (keyState.IsKeyDown(Keys.Subtract)){
                 _cameraZoom -= .01f;
             }


             if (keyState.IsKeyDown(Keys.Escape))
             {
                 _teddy.Dead = true;
             }

            if (_teddy.Dead)
            {
                _timerStarted = false;

                if (keyState.IsKeyDown(Keys.Enter))
                {
                    _userPressedEnterToGoBack = true;
                }

                if (_userPressedEnterToGoBack)
                {
                    if (keyState.IsKeyUp(Keys.Enter))
                    {
                        _goBackToStartScreen = true;
                    }
                }
            }
            else if (_teddy.LevelComplete)
            {
                _timerStarted = false;

                if (keyState.IsKeyDown(Keys.Enter))
                {
                    _userPressedEnterToGoBack = true;
                }

                if (_userPressedEnterToGoBack)
                {
                    if (keyState.IsKeyUp(Keys.Enter))
                    {
                        _goBackToStartScreen = true;
                    }
                }
            }
            else
            {
                _timerStarted = true;
            }

         

            foreach (GameObject f in GameObjects)
            {
                f.Update(gameTime);
            }

            foreach (Enemy bb in _enemies)
            {
                bb.Update(gameTime);
            }

            _teddy.Update(gameTime);

        }


        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            Matrix cameraView = SetCamera();


            //Begin all sprite batches
            _overlayBatch.Begin();
            _backgroundBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, cameraView);
            _playerBatch.Begin( SpriteSortMode.Deferred, null, null,null,null, null, cameraView   );
            _foregroundBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, cameraView);
            
            //Draw Background images
            foreach (Background b in this.Backgrounds)
            {
                Texture2D backgroundTexture;
                if (_backgroundImages.TryGetValue(b.BackgroundName, out backgroundTexture))
                {
                    DrawBackground(gameTime, b, backgroundTexture, _backgroundBatch, _foregroundBatch);
                }
            }

            //Draw foreground items
            foreach (Surface sur in Surfaces)
            {
              //  if ( _cameraBounds.Intersects( sur.Rect )){
                    Texture2D surfaceTexture;
                    if (_surfaceTextures.TryGetValue( sur.Sprite, out surfaceTexture)) {
                        DrawSurface(sur.Rect.X, sur.Rect.Y, sur.Rect.Width, sur.Rect.Height, surfaceTexture, _playerBatch )  ;
                    }
              //  }
            }

            foreach (GameObject f in GameObjects)
            {
              //  if (_cameraBounds.Intersects( f.CollisionRectangle ) )
                    f.Draw(gameTime, _playerBatch);
            }

            foreach (Enemy bb in _enemies)
            {
                //todo: figure out why bowling ball collision doesn't work with this
              //  if ( _cameraBounds.Intersects( bb.CollisionRectangle ) )
                    bb.DrawEnemy(gameTime, _playerBatch);
            }


            Rectangle r;

            if (_teddy.Dead)
            {
                Color c = new Color(255, 255, 255, _deathScreenCounter);

                r = new Rectangle(0, 0, _deathSprite.Width, _deathSprite.Height);
                _overlayBatch.Draw(_deathSprite, r, c);
                _overlayBatch.DrawString(_deathFont, "Press Enter To Continue", new Vector2(625, 500), Color.White);

                if (_deathScreenCounter < 255)
                {
                    _deathScreenCounter+=3;
                }

            }
            else if (_teddy.LevelComplete)
            {
                Color c = new Color(255, 255, 255, _deathScreenCounter);

                r = new Rectangle(0, 0, _successSprite.Width, _successSprite.Height);
                _overlayBatch.Draw(_successSprite, r, c);
                _overlayBatch.DrawString(_deathFont, "Press Enter To Continue", new Vector2(625, 500), Color.White);
                _overlayBatch.DrawString(_deathFont, "You completed this level in " + MilliSecondsElapsed.ToString() + " seconds!", new Vector2(625, 525), Color.White);

                if (_deathScreenCounter < 255)
                {
                    _deathScreenCounter += 3;
                }
            }

            if (_levelType == LevelType.Dark && _teddy._currentPowerup == null)
            {
                _overlayBatch.Draw(_overlaySprite, new Rectangle(0, 0, _overlaySprite.Width, _overlaySprite.Height), new Color(Color.Gray.R, Color.Gray.G, Color.Gray.B, 248));
            }

            if (_teddy._currentPowerup != null)
            {
                _teddy._currentPowerup.ScreenDraw(gameTime, _overlayBatch, _teddy, SpriteEffects.None);
            }

            _teddy.Draw(gameTime, _playerBatch);


            _backgroundBatch.End();
            _playerBatch.End();

            _foregroundBatch.End();
            _overlayBatch.End();

        }

        private void DrawSurface(int x, int y, int totalWidth, int totalHeight, Texture2D surfaceTexture, SpriteBatch surfaceBatch)
        {
            totalHeight = totalHeight / surfaceTexture.Height;
            totalWidth = totalWidth / surfaceTexture.Width;

            for (int i = 0; i < totalWidth; i++)
            {
                for (int j = 0; j < totalHeight; j++)
                {
                    if ((surfaceTexture.Width * i) + x < _cameraBounds.Right && (surfaceTexture.Width * i) + x > -surfaceTexture.Width)
                    {
                        surfaceBatch.Draw(surfaceTexture, new Rectangle((surfaceTexture.Width * i) + x, (surfaceTexture.Height * j) + y, surfaceTexture.Width, surfaceTexture.Height), Color.White);
                    }

                }
            }
        }

        private void DrawBackground(GameTime gameTime, Background b, Texture2D backgroundTexture, SpriteBatch backgroundBatch, SpriteBatch foregroundBatch)
        {
            int numViewsX = 1;
            int numViewsY = 1;


            Vector2 hoverOffset = b.GetHoverOffset(gameTime, backgroundTexture);

            Rectangle paddedBackground = new Rectangle(0, 0, backgroundTexture.Width + (int)b.Offset.X, backgroundTexture.Height + (int)b.Offset.Y);
            Rectangle r;

            if (b.RepeatX)
                numViewsX = (_cameraBounds.Width / paddedBackground.Width) + 2;

            if (b.RepeatY)
                numViewsY = (_cameraBounds.Height / paddedBackground.Height) + 2;

            //Scrolling doesn't make sense without repeat?
            if (b.Scrolls)
            {
                if (b.RepeatX || b.RepeatY)
                {
                    //Repeat this texture to fill the screen.  It scrolls with the level

                    //based on GlobalPosition, figure out where to draw this sprite  (and how many times)
                    int backgroundOffsetX = (int)(((int)-_cameraBounds.X) % paddedBackground.Width);
                    int backgroundOffsetY = (int)(((int)-_cameraBounds.Y) % paddedBackground.Height);

                    if (!b.RepeatY)
                        backgroundOffsetY = 0;

                    if (!b.RepeatX)
                        backgroundOffsetX = 0;

                    for (int i = -1; i <= numViewsX; i++)
                    {
                        for (int j = 0; j < numViewsY; j++)
                        {
                            int startX =  _cameraBounds.X + i * paddedBackground.Width + backgroundOffsetX + (int)b.Offset.X + (int)hoverOffset.X;
                            int startY =  _cameraBounds.Y + (j) * paddedBackground.Height + backgroundOffsetY + (int)b.Offset.Y + (int)hoverOffset.Y;

                            r = new Rectangle(startX, startY , backgroundTexture.Width, backgroundTexture.Height);
                            if ( b.Foreground) 
                                foregroundBatch.Draw(backgroundTexture, r, Color.White);
                            else
                                backgroundBatch.Draw(backgroundTexture, r, Color.White);
                        }
                    }
                }
                else
                {
                    //Only draw if it is actually visible
                    if (_cameraBounds.X + paddedBackground.Width > 0)
                    {
                        for (int j = 0; j < numViewsY; j++)
                        {
                            int startX = (int)b.Offset.X + (int)hoverOffset.X;
                            int startY = j * paddedBackground.Height + (int)b.Offset.Y + (int)hoverOffset.Y;

                            r = new Rectangle(startX, startY, backgroundTexture.Width, backgroundTexture.Height);
                            if ( b.Foreground) 
                                foregroundBatch.Draw(backgroundTexture, r, Color.White);
                            else
                                backgroundBatch.Draw(backgroundTexture, r, Color.White);
                        }
                    }
                }
            }
            else
            {
                //Repeat this texture to fill the screen.  It always stays in the same position relative to the viewport
                for (int i = 0; i < numViewsX; i++)
                {
                    for (int j = 0; j < numViewsY; j++)
                    {

                        int startX =  _cameraBounds.X + i * paddedBackground.Width + (int)b.Offset.X + (int)hoverOffset.X;
                        int startY =  _cameraBounds.Y + j * paddedBackground.Height + (int)b.Offset.Y + (int)hoverOffset.Y;

                        r = new Rectangle(startX,startY, backgroundTexture.Width, backgroundTexture.Height);
                        if ( b.Foreground) 
                            foregroundBatch.Draw(backgroundTexture, r, Color.White);
                        else
                            backgroundBatch.Draw(backgroundTexture, r, Color.White);
                    }
                }
            }
        }





        private Matrix SetCamera(){

            if (_teddy.LevelComplete)
                return Matrix.CreateTranslation(_currentCamera.X, _currentCamera.Y, 0); 

            Matrix cameraView;

            float cameraX;
            float cameraY;
            cameraY = 0;
            cameraX = 0;

            int teddyTestPositionX = (int)Math.Floor( _teddy.Position.X + _teddy.TeddyRectangle.Width / 2);
            int teddyTestPositionY = (int)Math.Floor( _teddy.Position.Y + _teddy.TeddyRectangle.Height / 2);


            if (_cameraZoom == 1.0)
            {
                if (teddyTestPositionX < Game.GraphicsDevice.Viewport.Width / 2)
                    cameraX = 0;
                else if (teddyTestPositionX > _totalLevelWidth - Game.GraphicsDevice.Viewport.Width / 2)
                    cameraX = -_totalLevelWidth + (Game.GraphicsDevice.Viewport.Width);
                else
                    cameraX = -teddyTestPositionX + (Game.GraphicsDevice.Viewport.Width / 2);


                if (teddyTestPositionY < (Game.GraphicsDevice.Viewport.Height - 100) / 2)
                    cameraY = 0;
                else if (teddyTestPositionY > _totalLevelHeight - (Game.GraphicsDevice.Viewport.Height - 100) / 2)
                    cameraY = -_totalLevelHeight + (Game.GraphicsDevice.Viewport.Height - 100);
                else
                    cameraY = -teddyTestPositionY + ((Game.GraphicsDevice.Viewport.Height - 100) / 2);


            }
            else
            {
                //Center around teddy
                cameraX = -teddyTestPositionX + (Game.GraphicsDevice.Viewport.Width / 2) / _cameraZoom;
                cameraY = -teddyTestPositionY + (Game.GraphicsDevice.Viewport.Height / 2) / _cameraZoom;


            }




            _currentCamera = new Vector2(cameraX, cameraY);

            cameraView = Matrix.CreateTranslation(_currentCamera.X, _currentCamera.Y, 0);
            cameraView *=   Matrix.CreateScale(_cameraZoom);

            _cameraBounds = new Rectangle(-(int)_currentCamera.X, (int)_currentCamera.Y, Game.GraphicsDevice.Viewport.Width, (Game.GraphicsDevice.Viewport.Height - 100));

            return cameraView;

        }
    }





        public interface ISurfaceInterface
    {
        Rectangle SurfaceBounds();
         Vector2 SurfaceVelocity();
         Enemy SurfaceOwner();
               
    }

}
