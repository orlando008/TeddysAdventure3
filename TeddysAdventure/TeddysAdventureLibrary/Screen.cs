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
        private Vector2 _globalPosition;
        private List<Surface> _surfaces;
        private ScreenHelper _screenHelper;
        private List<Texture2D> _sprites;
        private int _totalLevelWidth;
        private List<GameObject> _gameObjects;
        private List<Enemy> _enemies;
        private int _deathScreenCounter = 0;
        private bool _goBackToStartScreen = false;
        private bool _userPressedEnterToGoBack = false;
        private LevelType _levelType = LevelType.Normal;
        private SpriteFont _deathFont;
        private SpriteFont _hudFont;
        private Dictionary<string,Texture2D> _surfaceTextures;

        public static SpriteBatch spriteBatch;

        public List<Surface> Surfaces
        {
            get { return _surfaces; }
            set { _surfaces = value; }
        }

        public List<Texture2D> Sprites
        {
            get { return _sprites; }
            set { _sprites = value; }
        }


        public Vector2 GlobalPosition
        {
            get { return _globalPosition; }
            set { _globalPosition = value; }
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

        public bool GoBackToStartScreen
        {
            get { return _goBackToStartScreen; }
            set { _goBackToStartScreen = value; }
        }

        public Screen(Game game, string levelName)
            : base(game)
        {
            _screenHelper = game.Content.Load<ScreenHelper>("Screens\\" + levelName);
            
            _deathSprite = game.Content.Load<Texture2D>("Screens\\deathScreen");
            _deathFont = game.Content.Load<SpriteFont>("Fonts\\DeathScreenFont");
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\HudFont");

            _surfaceTextures = new Dictionary<string, Texture2D>();
            _surfaces = new List<Surface>();

            var allScreenSprites = from s in _screenHelper.Surfaces select s.Sprite;

            foreach (SurfaceHelper sh in _screenHelper.Surfaces)
            {
                if(!_surfaceTextures.ContainsKey(sh.Sprite))
                    _surfaceTextures.Add( sh.Sprite, game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", sh.Sprite)));

                _surfaces.Add(  new Surface(sh));

            }

             
            GlobalPosition = new Vector2(0, 0);

            GameObjects = new List<GameObject>();
            foreach (GameObjectHelper v2 in _screenHelper.ListOfObjects)
            {
                switch (v2.Type)
                {
                    case "Fluff":
                        GameObjects.Add(new Fluff(game, v2.Position));
                        break;
                }
                
            }

            _totalLevelWidth = (int)_screenHelper.LevelSize.X;
            _enemies = new List<Enemy>();

            foreach (EnemyHelper eh in _screenHelper.ListOfEnemies)
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

            switch (_screenHelper.LevelType)
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

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }



        /// <summary>
        /// Is teddy at the last screen of the level?
        /// </summary>
        /// <param name="pixelsOver">pixelsOver tells you how many pixels the screen has moved past its end. You can use this to move the screen back to a proper location.</param>
        /// <returns>Returns true if this is the last screen of the level</returns>
        public bool TeddyAtLastScreen(ref int pixelsOver)
        {
            int sum = 0;

            sum = _totalLevelWidth - Game.GraphicsDevice.Viewport.Width;

            int difference = (int)GlobalPosition.X + sum;

            if (difference <= 0)
            {
                pixelsOver = difference;
                return true;
            }
            else
            {
                pixelsOver = 0;
                return false;
            }
        }

        //Move the screen in the X direction
        public void MoveX(int speed)
        {
            //move all the surfaces
            for (int i = Surfaces.Count - 1; i >= 0 ; i--)
            {
                Surfaces[i].MoveByX(speed);

            }

            //move all the fluffs
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].MoveGameObjectByX(speed);
            }

            //move all the enemies
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].MoveByX(speed,true);
            }

            //Set the overall global position
            GlobalPosition = new Vector2(GlobalPosition.X + speed, GlobalPosition.Y);
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


            foreach (Fluff f in GameObjects)
            {
                f.Update(gameTime);
            }

            foreach (Enemy bb in _enemies)
            {
                bb.Update(gameTime);
            }

        }

        private void DrawSurface(int x, int y, int totalWidth, int totalHeight, Texture2D surfaceTexture)
        {
            totalHeight = totalHeight / surfaceTexture.Height;
            totalWidth = totalWidth / surfaceTexture.Width;

            for (int i = 0; i < totalWidth; i++)
            {
                for (int j = 0; j < totalHeight; j++)
                {
                    if ((surfaceTexture.Width * i) + x < Game.GraphicsDevice.Viewport.Width && (surfaceTexture.Width * i) + x > -surfaceTexture.Width)
                    {
                        spriteBatch.Draw(surfaceTexture, new Rectangle((surfaceTexture.Width * i) + x, (surfaceTexture.Height * j) + y, surfaceTexture.Width, surfaceTexture.Height), Color.White);
                    }
                    
                }
            }
        }

        public void Draw(GameTime gameTime)
        {


            spriteBatch.Begin();

            Rectangle r;

            foreach (Surface sur in Surfaces)
            {
                Texture2D surfaceTexture;
                if (_surfaceTextures.TryGetValue( sur.Sprite, out surfaceTexture)) {
                    DrawSurface(sur.Rect.X, sur.Rect.Y, sur.Rect.Width, sur.Rect.Height, surfaceTexture )  ;
                }
            }

            foreach (GameObject f in GameObjects)
            {
                f.Draw(gameTime, spriteBatch);
            }

            foreach (Enemy bb in _enemies)
            {
                bb.DrawEnemy(gameTime, spriteBatch);
            }

            spriteBatch.DrawString(_hudFont, "Fluff Count: " + ((Teddy)Game.Components[1]).CurrentFluff.ToString(), new Vector2(25, 700), Color.LightBlue);
            spriteBatch.DrawString(_hudFont, "Enemies Destroyed: " + ((Teddy)Game.Components[1]).EnemiesDestroyed.ToString(), new Vector2(25, 725), Color.LightBlue);


            if (((Teddy)Game.Components[1]).Dead)
            {
                Color c = new Color(255, 255, 255, _deathScreenCounter);

                r = new Rectangle(0, 0, _deathSprite.Width, _deathSprite.Height);
                spriteBatch.Draw(_deathSprite, r, c);

                spriteBatch.DrawString(_deathFont, "Press Enter To Continue", new Vector2(625, 500), Color.White);

                if (_deathScreenCounter < 255)
                {
                    _deathScreenCounter+=3;
                }

            }

            spriteBatch.End();


        }
    }

    public interface ISurfaceInterface
    {
        Rectangle SurfaceBounds();
         Vector2 SurfaceVelocity();
               
    }

}
