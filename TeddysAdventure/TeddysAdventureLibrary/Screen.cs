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
        private List<Rectangle> _surfaces;
        private ScreenHelper _screenHelper;
        private List<Texture2D> _sprites;
        private List<Vector2> _positions;
        private int _totalLevelWidth;
        private List<Fluff> _fluffs;
        private List<Enemy> _enemies;
        private int _deathScreenCounter = 0;
        private bool _goBackToStartScreen = false;
        private bool _userPressedEnterToGoBack = false;
        private LevelType _levelType = LevelType.Normal;
        private SpriteFont _deathFont;
        private SpriteFont _hudFont;
        private Texture2D _surfaceTexture;

        public static SpriteBatch spriteBatch;

        public List<Rectangle> Surfaces
        {
            get { return _surfaces; }
            set { _surfaces = value; }
        }

        public List<Texture2D> Sprites
        {
            get { return _sprites; }
            set { _sprites = value; }
        }

        public List<Vector2> Positions
        {
            get { return _positions; }
            set { _positions = value; }
        }

        public Vector2 GlobalPosition
        {
            get { return _globalPosition; }
            set { _globalPosition = value; }
        }

        public List<Fluff> Fluffs
        {
            get { return _fluffs; }
            set { _fluffs = value; }
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
            
            Sprites = new List<Texture2D>();
            foreach (string s in _screenHelper.Assets)
            {
                Sprites.Add( game.Content.Load<Texture2D>("Screens\\" + s));
                _totalLevelWidth += Sprites[Sprites.Count - 1].Width;
            }

            _deathSprite = game.Content.Load<Texture2D>("Screens\\deathScreen");
            _deathFont = game.Content.Load<SpriteFont>("Fonts\\DeathScreenFont");
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\HudFont");

            _surfaceTexture = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", "SurfaceTexture1"));

            Positions = new List<Vector2>();

            foreach (int i in _screenHelper.Positions)
            {
                Positions.Add(new Vector2(i, 0));
            }
            GlobalPosition = new Vector2(Positions[0].X, Positions[0].Y);

            Fluffs = new List<Fluff>();
            foreach (Vector2 v2 in _screenHelper.FluffLocations)
            {
                Fluffs.Add(new Fluff(game, v2));
            }

            _enemies = new List<Enemy>();

            foreach (EnemyHelper eh in _screenHelper.ListOfEnemies)
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
                    case "Plane":
                        _enemies.Add(new PlaneEnemy(game, eh.Position, eh.Velocity));
                        break;
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

            Surfaces = _screenHelper.Surfaces;
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
                Surfaces[i] = new Rectangle(Surfaces[i].X + speed, Surfaces[i].Y, Surfaces[i].Width, Surfaces[i].Height); 
            }

            //move all the positions
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i] = new Vector2(Positions[i].X + speed, Positions[i].Y);
            }

            //move all the fluffs
            for (int i = 0; i < Fluffs.Count; i++)
            {
                Fluffs[i].MoveFluffByX(speed);
            }

            //move all the enemies
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].MoveByX(speed);
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


            foreach (Fluff f in Fluffs)
            {
                f.Update(gameTime);
            }

            foreach (Enemy bb in _enemies)
            {
                bb.Update(gameTime);
            }

        }

        private void DrawSurface(int x, int y, float totalWidth, float totalHeight, Texture2D surfaceTexture)
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

            foreach (Rectangle sur in Surfaces)
            {
                DrawSurface(sur.X, sur.Y, sur.Width, sur.Height, _surfaceTexture);
            }

            foreach (Fluff f in Fluffs)
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
