using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class Eagle:Enemy
    {
        public Eagle(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\Eagle");

            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            base.Velocity = velocity;
            this._collisionDampingFactor = .3f;
            this._gravity = 0f;
            CanJumpOnToKill = true;
            this._passesThroughSurfaces = true;
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            if (this.Destroyed)
                return;

            SpriteEffects seff = SpriteEffects.None;
            if (Velocity.X < 0)
            {
                seff = SpriteEffects.FlipHorizontally;
            }

            sp.Draw(StyleSheet, CollisionRectangle, this.BoxToDraw, Color.White, 0, Vector2.Zero, seff, 0);
        }

    }
}
