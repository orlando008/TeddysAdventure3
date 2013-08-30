using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class LadyBug : Enemy
    {
        private int _framesOfPose = 10;
        public LadyBug(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\LadyBug");
          
            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            base.Velocity = velocity;
            CanJumpOnToKill = true;
            this._gravity = 0;
            this._changeDirectionUponSurfaceHit = true;
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            base.DrawEnemy(gameTime, sp);

            if (!Destroyed)
            {
              
                if (_frameCount < _framesOfPose)
                {
                    sp.Draw(this.StyleSheet, CollisionRectangle, this.BoxToDraw, Color.White, 0, new Vector2(0,0), SpriteEffects.FlipHorizontally, 0);
                    _frameCount++;
                }
                else if (_frameCount < _framesOfPose * 2)
                {
                    sp.Draw(this.StyleSheet, CollisionRectangle, this.BoxToDraw, Color.White);
                    _frameCount++;
                }
                else
                {
                    sp.Draw(this.StyleSheet, CollisionRectangle, this.BoxToDraw, Color.White);
                    _frameCount = 0;
                }
                

            }
        }
    }
}
