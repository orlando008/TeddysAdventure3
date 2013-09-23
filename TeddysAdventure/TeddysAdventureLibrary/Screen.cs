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
            Falling = 3
        }

        private Texture2D _deathSprite;
        private Texture2D _successSprite;
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

        private int _deathScreenCounter = 0;
        private bool _goBackToStartScreen = false;
        private bool _userPressedEnterToGoBack = false;

        private Color _backgroundColor;

        public static SpriteBatch _backgroundBatch;
        public static SpriteBatch _foregroundBatch;
        public static SpriteBatch _overlayBatch;

        private Vector2 _currentCamera;
        private Rectangle _cameraBounds;


        public Rectangle CameraBounds { get { return _cameraBounds; } }

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

        public Screen(Game game, string levelName, Teddy teddy)
            : base(game)
        {


            ScreenHelper screenHelper = game.Content.Load<ScreenHelper>("Screens\\" + levelName);
            _teddy = teddy;
            _deathSprite = game.Content.Load<Texture2D>("Screens\\deathScreen");
            _successSprite = game.Content.Load<Texture2D>("Screens\\successScreen");
            _deathFont = game.Content.Load<SpriteFont>("Fonts\\DeathScreenFont");
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\HudFont");

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
                if(!_surfaceTextures.ContainsKey(sh.Sprite))
                    _surfaceTextures.Add( sh.Sprite, game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", sh.Sprite)));

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
                        case "LadyBug":
                            _enemies.Add(new LadyBug(game, eh.Position, eh.Velocity));
                            break;
                        case "OrangeBomb":
                            _enemies.Add(new OrangeBomb(Game, eh.Position, eh.Velocity));
                            break;
                        case "OrangeCrayon":
                            _enemies.Add(new OrangeCrayon(Game, eh.Position, eh.Velocity));
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
                default:
                    _levelType = LevelType.Normal;
                    break;
            }

            _foregroundBatch = new SpriteBatch(Game.GraphicsDevice);
            _overlayBatch = new SpriteBatch(Game.GraphicsDevice);
            _backgroundBatch = new SpriteBatch(game.GraphicsDevice);
        }


        public override void Update(GameTime gameTime)
        {
            if (((Teddy)Game.Components[1]).Dead)
            {
                KeyboardState keyState = Keyboard.GetState();

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
            else if (((Teddy)Game.Components[1]).LevelComplete)
            {
                KeyboardState keyState = Keyboard.GetState();

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


            foreach (GameObject f in GameObjects)
            {
                f.Update(gameTime);
            }

            foreach (Enemy bb in _enemies)
            {
                bb.Update(gameTime);
            }

        }


        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            Matrix cameraView = SetCamera();


            //Begin all sprite batches
            _overlayBatch.Begin();
            _backgroundBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, cameraView);
            _foregroundBatch.Begin( SpriteSortMode.Deferred, null, null,null,null, null, cameraView   );

            
            //Draw Background images
            foreach (Background b in this.Backgrounds)
            {
                Texture2D backgroundTexture;
                if (_backgroundImages.TryGetValue(b.BackgroundName, out backgroundTexture))
                {
                    DrawBackground(b, backgroundTexture, _backgroundBatch);
                }
            }

            //Draw foreground items
            foreach (Surface sur in Surfaces)
            {
              //  if ( _cameraBounds.Intersects( sur.Rect )){
                    Texture2D surfaceTexture;
                    if (_surfaceTextures.TryGetValue( sur.Sprite, out surfaceTexture)) {
                        DrawSurface(sur.Rect.X, sur.Rect.Y, sur.Rect.Width, sur.Rect.Height, surfaceTexture, _foregroundBatch )  ;
                    }
              //  }
            }

            foreach (GameObject f in GameObjects)
            {
              //  if (_cameraBounds.Intersects( f.CollisionRectangle ) )
                    f.Draw(gameTime, _foregroundBatch);
            }

            foreach (Enemy bb in _enemies)
            {
                //todo: figure out why bowling ball collision doesn't work with this
              //  if ( _cameraBounds.Intersects( bb.CollisionRectangle ) )
                    bb.DrawEnemy(gameTime, _foregroundBatch);
            }

            _overlayBatch.DrawString(_hudFont, "Fluff Count: " + _teddy.CurrentFluff.ToString(), new Vector2(25, 700), Color.LightBlue);
            _overlayBatch.DrawString(_hudFont, "Enemies Destroyed: " + _teddy.EnemiesDestroyed.ToString(), new Vector2(25, 725), Color.LightBlue);


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

                if (_deathScreenCounter < 255)
                {
                    _deathScreenCounter += 3;
                }
            }

            _teddy.Draw(gameTime, _foregroundBatch);


            _backgroundBatch.End();
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

        private void DrawBackground(Background b, Texture2D backgroundTexture, SpriteBatch backgroundBatch)
        {
            int numViewsX = 1;
            int numViewsY = 1;

            Rectangle paddedBackground = new Rectangle(0, 0, backgroundTexture.Width + (int)b.Offset.X, backgroundTexture.Height + (int)b.Offset.Y);
            Rectangle r;

            if (b.RepeatX)
                numViewsX = (Game.GraphicsDevice.Viewport.Width / paddedBackground.Width) + 1;

            if (b.RepeatY)
                numViewsY = (Game.GraphicsDevice.Viewport.Height / paddedBackground.Height) + 1;

            //Scrolling doesn't make sense without repeat?
            if (b.Scrolls)
            {
                if (b.RepeatX)
                {
                    //Repeat this texture to fill the screen.  It scrolls with the level

                    //based on GlobalPosition, figure out where to draw this sprite  (and how many times)
                    int backgroundOffsetX = (int)(((int)_cameraBounds.X) % paddedBackground.Width);
                    int backgroundOffsetY = (int)(((int)_cameraBounds.Y) % paddedBackground.Height);

                    for (int i = 0; i <= numViewsX; i++)
                    {
                        for (int j = 0; j < numViewsY; j++)
                        {
                            r = new Rectangle( _cameraBounds.X + i * paddedBackground.Width - backgroundOffsetX + (int)b.Offset.X, j * paddedBackground.Height + backgroundOffsetY + (int)b.Offset.Y, backgroundTexture.Width, backgroundTexture.Height);
                            backgroundBatch.Draw(backgroundTexture, r, Color.White);
                        }
                    }

                }
                else if (b.RepeatY)
                {
                    //Only draw if it is actually visible
                    if (_cameraBounds.X + paddedBackground.Width > 0)
                    {
                        for (int j = 0; j < numViewsY; j++)
                        {

                            r = new Rectangle((int)_cameraBounds.X + (int)b.Offset.X, j * paddedBackground.Height + (int)b.Offset.Y, backgroundTexture.Width, backgroundTexture.Height);
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

                            r = new Rectangle( (int)b.Offset.X, j * paddedBackground.Height + (int)b.Offset.Y, backgroundTexture.Width, backgroundTexture.Height);
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
                        r = new Rectangle(i * paddedBackground.Width + (int)b.Offset.X, j * paddedBackground.Height + (int)b.Offset.Y, backgroundTexture.Width, backgroundTexture.Height);
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

            if (_teddy.Position.X < Game.GraphicsDevice.Viewport.Width / 2)
                cameraX = 0;
            else if (_teddy.Position.X > _totalLevelWidth - Game.GraphicsDevice.Viewport.Width / 2)
                cameraX = -_totalLevelWidth + (Game.GraphicsDevice.Viewport.Width);
            else
                cameraX = -_teddy.Position.X + (Game.GraphicsDevice.Viewport.Width / 2);


            if (_teddy.Position.Y < Game.GraphicsDevice.Viewport.Height / 2)
                cameraY = 0;
            else if (_teddy.Position.Y > _totalLevelHeight - Game.GraphicsDevice.Viewport.Height / 2)
                cameraY = -_totalLevelHeight + (Game.GraphicsDevice.Viewport.Height);
            else
                cameraY = -_teddy.Position.Y + (Game.GraphicsDevice.Viewport.Height / 2);


            _currentCamera = new Vector2(cameraX, cameraY);

            cameraView = Matrix.CreateTranslation(_currentCamera.X, _currentCamera.Y, 0);

            _cameraBounds = new Rectangle(-(int)_currentCamera.X, (int)_currentCamera.Y, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);

            return cameraView;

        }


    }





        public interface ISurfaceInterface
    {
        Rectangle SurfaceBounds();
         Vector2 SurfaceVelocity();
               
    }

}
