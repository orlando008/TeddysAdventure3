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
    public class StartMenu : DrawableGameComponent
    {
        private Texture2D sprite;
        private Texture2D _cursorSprite;
        private Vector2 _position;
        private SpriteFont _hudFont;

        private int _selectedLevel = 0;

        private Boolean _started = false;

        public static SpriteBatch spriteBatch;

        private List<string> _levelsList;

        public string SelectedLevelName
        {
            get
            {
                if (_levelsList.Count > _selectedLevel)
                    return _levelsList[_selectedLevel];
                else
                    return string.Empty;
            }
        }

        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public Texture2D CursorSprite
        {
            get { return _cursorSprite; }
            set { _cursorSprite = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Boolean Started
        {
            get { return _started; }
            set { _started = value; }
        }

        public StartMenu(Game game, Vector2 position, Texture2D myStyleSheet)
            : base(game)
        {
            Position = position;
            Sprite = myStyleSheet;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _levelsList = LoadLevelList(game);
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\HudFont");
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            Rectangle r = new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height);

            spriteBatch.Draw(Sprite, r, Color.White);


            for (int i = 0; i < _levelsList.Count; i++)
            {
                string level = _levelsList[i];
                var stringPosition = new Vector2( 5, 20 * i );

                spriteBatch.DrawString( _hudFont, string.Format( "    [{0}]:  {1}", i, level ), stringPosition  , Color.White );

                if(i == _selectedLevel ) 
                    spriteBatch.DrawString( _hudFont, "*", new Vector2(0, 20*i + 5)  , Color.White );

            }


            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Enter))
            {
                _started = true;
            }

            if (keyState.IsKeyDown(Keys.Up))
                _selectedLevel -= 1;

            if (keyState.IsKeyDown(Keys.Down))
                _selectedLevel += 1;

            if (keyState.IsKeyDown(Keys.D0))
                _selectedLevel = 0;

            if (keyState.IsKeyDown(Keys.D1))
                _selectedLevel = 1;

            if (keyState.IsKeyDown(Keys.D2))
                _selectedLevel = 2;

            if (keyState.IsKeyDown(Keys.D3))
                _selectedLevel = 3;

            if (keyState.IsKeyDown(Keys.D4))
                _selectedLevel = 4;

            if (keyState.IsKeyDown(Keys.D5))
                _selectedLevel = 5;

            if (keyState.IsKeyDown(Keys.D6))
                _selectedLevel = 6;

            if (keyState.IsKeyDown(Keys.D7))
                _selectedLevel = 7;

            if (keyState.IsKeyDown(Keys.D8))
                _selectedLevel = 8;

            if (keyState.IsKeyDown(Keys.D9))
                _selectedLevel = 9;


            if (_selectedLevel < 0)
                _selectedLevel = 0;

            if (_selectedLevel >= _levelsList.Count)
                _selectedLevel = _levelsList.Count - 1;

        }


        private List<string> LoadLevelList(Game game)
        {
            var levels = new List<string>();

            // load our manifest so we know what files we have
            List<string> contentFiles = game.Content.Load<List<string>>("content"); 

            IEnumerable<string> levelContent = from file in contentFiles where file.Contains("Screens\\") && file.Contains(".xml") select file.Substring( 8).Replace(".xml", string.Empty) ;

            return levelContent.ToList<string>();
        }

    }
}
