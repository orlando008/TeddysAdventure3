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
    public class HUD : DrawableGameComponent
    {
        private SpriteFont _font12;
        private SpriteFont _font10;
        private SpriteFont _font14;

        private Color _fontColor = Color.Black;
        private Color _backgroundColor = Color.Black;
        private Vector2 _globalPosition;
        private String _levelName;
        private Texture2D _hudTexture;
        private Texture2D _teddyLife;
        private Texture2D _fluffIcon;

        private Vector2 _positionOfLives = new Vector2(25, 760);
        private Vector2 _positionOfFluffs = new Vector2(25, 800);

        //icon

        public static SpriteBatch spriteBatch;

        public HUD(Game game, string levelName)
            : base(game)
        {
            _font12 = game.Content.Load<SpriteFont>("Fonts\\Arial12");
            _font10 = game.Content.Load<SpriteFont>("Fonts\\Arial10");
            _font14 = game.Content.Load<SpriteFont>("Fonts\\Arial14");

            _levelName = levelName;

            _hudTexture = game.Content.Load<Texture2D>("Screens\\Backgrounds\\HUDCloud");
            _teddyLife = game.Content.Load<Texture2D>("Icons\\TeddyLife");
            _fluffIcon = game.Content.Load<Texture2D>("Icons\\FluffIcon");

            _globalPosition = new Vector2(0, 750); // Starts at 750 down
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();


            spriteBatch.Draw(_hudTexture, new Vector2(0, 750), Color.White);



            foreach (Enemy e in ((Screen)Game.Components[0]).Enemies)
	        {
                if (e.HudIcon != null && e.BossIconObject == null)
                {
                    e.BossIconObject = new BossIcon(Game, e, new Vector2(800, _globalPosition.Y + 5), _font10, _font10);
                }

                if(e.BossIconObject != null && !e.Destroyed)
                    e.BossIconObject.Draw(gameTime, spriteBatch);
	        }


            spriteBatch.Draw(_teddyLife, _positionOfLives, Color.White);
            spriteBatch.DrawString(_font14, " x " + ((Screen)Game.Components[0]).Teddy.CurrentLives.ToString(), new Vector2(_positionOfLives.X + _teddyLife.Width, _positionOfLives.Y), _fontColor);


            spriteBatch.Draw(_fluffIcon, _positionOfFluffs, Color.White);
            spriteBatch.DrawString(_font14, " x " + ((Screen)Game.Components[0]).Teddy.CurrentFluff.ToString(), new Vector2(_positionOfFluffs.X + _fluffIcon.Width, _positionOfFluffs.Y), _fontColor);

            //spriteBatch.DrawString(_hudFont, "Fluff Count: " + ((Screen)Game.Components[0]).Teddy.CurrentFluff.ToString(), new Vector2(25, 775), _fontColor);
            //spriteBatch.DrawString(_hudFont, "Enemies Destroyed: " + ((Screen)Game.Components[0]).Teddy.EnemiesDestroyed.ToString(), new Vector2(25, 800), _fontColor);
            //spriteBatch.DrawString(_hudFont, "Level: " + _levelName, new Vector2(500, 775), _fontColor);

            
            spriteBatch.End();
        }
    }
}
