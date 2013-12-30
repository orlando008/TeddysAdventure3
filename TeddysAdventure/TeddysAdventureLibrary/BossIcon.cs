using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TeddysAdventureLibrary
{
    public class BossIcon : DrawableGameComponent
    {
        private Enemy _enemy;
        private Vector2 _position;
        private SpriteFont _nameFont;
        private SpriteFont _healthFont;
        private Vector2 _sizeOfNameText;
        private Texture2D _healthBar;
        private Texture2D _healthUnit;

        public BossIcon(Game game, Enemy e, Vector2 position, SpriteFont nameFont, SpriteFont healthFont)
            : base(game)
        {
            _enemy = e;
            _position = position;
            _nameFont = nameFont;
            _healthFont = healthFont;
            _sizeOfNameText = _nameFont.MeasureString(_enemy.Name);
            _healthBar = game.Content.Load<Texture2D>("Icons\\HealthBar");
            _healthUnit = game.Content.Load<Texture2D>("Icons\\HealthUnit");
        }

        public void Draw(GameTime gameTime, SpriteBatch sp)
        {
            sp.Draw(_enemy.HudIcon, _position, new Rectangle(0, 0, _enemy.HudIcon.Width, _enemy.HudIcon.Height), Color.White);
            sp.DrawString(_nameFont, _enemy.Name, new Vector2(_position.X + (_enemy.HudIcon.Width - _sizeOfNameText.X) / 2, _position.Y + _enemy.HudIcon.Height), Color.DarkBlue);

            Vector2 sizeOfHealthText = _healthFont.MeasureString("Health: " + _enemy.Health.ToString());

            //sp.DrawString(_healthFont, "Health: " + _enemy.Health.ToString(), new Vector2(_position.X + (_enemy.HudIcon.Width - sizeOfHealthText.X) / 2, _position.Y + _enemy.HudIcon.Height + _sizeOfNameText.Y), Color.Red);
            
            sp.Draw(_healthBar, new Vector2(_position.X + (_enemy.HudIcon.Width - _healthBar.Width) / 2, _position.Y + _enemy.HudIcon.Height + _sizeOfNameText.Y), Color.White);

            int healthPercent = Convert.ToInt32( (float)_enemy.Health / (float)_enemy.MaxHealth * 100f);
            for (int i = 0; i <= healthPercent; i++)
            {
                sp.Draw(_healthUnit, new Vector2(_position.X + i + (_enemy.HudIcon.Width - _healthBar.Width) / 2, _position.Y + _enemy.HudIcon.Height + _sizeOfNameText.Y + 1), Color.White);
            }
        }
    }
}
