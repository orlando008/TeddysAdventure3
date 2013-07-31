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

        private Texture2D _deathSprite;
        private Vector2 _globalPosition;
        private List<Rectangle> _surfaces;
        private ScreenHelper _screenHelper;
        private List<Texture2D> _sprites;
        private List<Vector2> _positions;
        private int _totalLevelWidth;
        private List<Fluff> _fluffs;
        private int _deathScreenCounter = 0;
        private int _deathScreenFrameCount = 25;

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

            //Set the overall global position
            GlobalPosition = new Vector2(GlobalPosition.X + speed, GlobalPosition.Y);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Fluff f in Fluffs)
            {
                f.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {


            spriteBatch.Begin();

            Rectangle r;
            
            //Draw each of the screens of the level
            //The Positions list should be in one to one correspondence with Sprites list
            for (int i = 0; i < Sprites.Count; i++)
            {
                r = new Rectangle((int)Positions[i].X, (int)Positions[i].Y, Sprites[i].Width, Sprites[i].Height);
                spriteBatch.Draw(Sprites[i], r, Color.White);
            }

            foreach (Fluff f in Fluffs)
            {
                f.Draw(gameTime, spriteBatch);

            }

            if (((Teddy)Game.Components[1]).Dead)
            {
                Color c = new Color(255, 255, 255, _deathScreenCounter);

                r = new Rectangle(0, 0, _deathSprite.Width, _deathSprite.Height);
                spriteBatch.Draw(_deathSprite, r, c);

                if (_deathScreenCounter < 255)
                {
                    _deathScreenCounter++;
                }

            }

            spriteBatch.End();


        }
    }
}
