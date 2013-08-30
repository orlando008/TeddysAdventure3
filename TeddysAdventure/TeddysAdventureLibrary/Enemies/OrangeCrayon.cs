using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class OrangeCrayon : Enemy
    {
        private int _lengthOfPose = 40;
        public OrangeCrayon(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\OrangeCrayon");

            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            base.Velocity = velocity;
            CanJumpOnToKill = true;

            this.ChildrenEnemies = new List<Enemy>();
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            Color enemyColor = Color.White;

            if (this.Dying)
            {
                enemyColor.A = (byte)((1 - ((float)_deathFrameCount / _deathFrames)) * 255);
                BoxToDraw = new Rectangle(0, 0, BoxToDraw.Width, BoxToDraw.Height);
            }


            if (!Destroyed)
            {

                sp.Draw(StyleSheet, Position, BoxToDraw, enemyColor);

                //_frameCount++;

            }

            foreach (Enemy e in ChildrenEnemies)
            {
                e.DrawEnemy(gameTime, sp);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Destroyed)
                return;


            if (((Teddy)Game.Components[1]).Position.X < this.Position.X && Velocity.X > 0)
            {
                Velocity = new Vector2(Velocity.X * -1, Velocity.Y);
            }
            else if (((Teddy)Game.Components[1]).Position.X > this.Position.X && Velocity.X < 0)
            {
                Velocity = new Vector2(Velocity.X * -1, Velocity.Y);
            }

            base.Update(gameTime);

            _frameCount++;

            if (_frameCount > 75)
            {
                this.ChildrenEnemies.Add(new OrangeBomb(Game, this.Position, new Vector2(0, 0)));
                _frameCount = 0;
            }

            foreach (Enemy e in ChildrenEnemies)
            {
                e.Update(gameTime);
            }

        }
    }
}
