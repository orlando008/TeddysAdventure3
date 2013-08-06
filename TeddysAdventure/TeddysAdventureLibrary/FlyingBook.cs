using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    /// <summary>
    /// This enemy floats in the air near the top of the screen and at random intervals drops "A-Bombs" down.
    /// </summary>
    public class FlyingBook : Enemy
    {
        private ABomb _aBomb;
        private SpriteFont _hudFont;

        public FlyingBook(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\FlyingBook");
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\HudFont");
            Position = position;
            BoxToDraw = new Rectangle(0, 0, 103, StyleSheet.Height);
            base.Velocity = velocity;
            CanJumpOnToKill = true;

            //Create the book's A-Bomb, put it off screen with no velocity
            _aBomb = new ABomb(game, new Vector2(-100, -100), new Vector2(0, 0));
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            if (!Destroyed)
            {
                sp.Draw(StyleSheet, Position, BoxToDraw, Color.White);  
            }
        }

        public override void Update(GameTime gameTime)
        {
  
        }
    }
}
