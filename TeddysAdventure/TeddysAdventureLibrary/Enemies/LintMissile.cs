using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    /// <summary>
    /// This gets dropped from the Flying Book Enemy, it just drops down from the sky
    /// </summary>
    public class LintMissile : Enemy
    {
        private Texture2D _missileOveryLay;
        private Rectangle _missileOverlayBoxToDraw;

        public LintMissile(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\VacuumLintMissile");
            _missileOveryLay = game.Content.Load<Texture2D>("Enemies\\VacuumLintMissileOverlay");

            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            _missileOverlayBoxToDraw = new Rectangle(0, 0, _missileOveryLay.Width / 2, _missileOveryLay.Height);

            base.Velocity = velocity;
            CanJumpOnToKill = false;
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            Color enemyColor = Color.White;

            if (_frameCount <= 10)
            {
                _missileOverlayBoxToDraw = new Rectangle(0, 0, _missileOveryLay.Width / 2, _missileOveryLay.Height);
            }
            else if (_frameCount <= 20)
            {
                _missileOverlayBoxToDraw = new Rectangle(_missileOveryLay.Width / 2, 0, _missileOveryLay.Width / 2, _missileOveryLay.Height);
            }
            else
            {
                _frameCount = 0;
            }

            if (this.Dying)
            {
                enemyColor.A = (byte)((1 - ((float)_deathFrameCount / _deathFrames)) * 255);
                BoxToDraw = new Rectangle(26, 0, 26, BoxToDraw.Height);
            }
            else
            {
                BoxToDraw = new Rectangle(0, 0, 26, BoxToDraw.Height);
            }

            if (!Destroyed)
            {

                sp.Draw(StyleSheet, Position, BoxToDraw, enemyColor);

                sp.Draw(_missileOveryLay, new Vector2(Position.X - 2.5f, Position.Y - 5f), _missileOverlayBoxToDraw, enemyColor);
                _frameCount++;

            }


        }

        public override void Update(GameTime gameTime)
        {
            if (!Dying)
            {
                Position = new Vector2(Position.X + Velocity.X, Position.Y + Velocity.Y);
            }

        }
    }
}
