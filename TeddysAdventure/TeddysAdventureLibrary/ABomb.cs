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
    public class ABomb : Enemy
    {
        public ABomb(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\ABomb");

            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            base.Velocity = velocity;
            CanJumpOnToKill = false;
            Kill();
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            Color enemyColor = Color.White;

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

                _frameCount++;

            }


        }

        public override void Update(GameTime gameTime)
        {
            if (!Dying)
            {
                Position = new Vector2(Position.X, Position.Y + Velocity.Y);
            }
            
        }
    }
}
