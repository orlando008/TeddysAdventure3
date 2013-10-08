using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    /// <summary>
    /// This is a simple enemy that moves back and forth on a platform
    /// </summary>
    public class DustBunny : Enemy
    {
        private int _lengthOfPose = 19;


        public DustBunny(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\DustBunny");

            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            base.Velocity = velocity;
            CanJumpOnToKill = true;
            this._collisionDampingFactor = 0.6f;
            this._fallsOffSurface = false;
            this._deathFrames = 20;
            this._changeDirectionUponSurfaceHit = true;
        }


        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            base.DrawEnemy(gameTime, sp);

            //For now i will just show text counting the frames

            Color enemyColor = Color.White;

            if (this.Dying)
                enemyColor.A = (byte)((1 - ((float)_deathFrameCount / _deathFrames)) * 255);

            
            if (!Destroyed)
            {
                if (_frameCount < _lengthOfPose * 1)
                {
                    BoxToDraw = new Rectangle(0, 0, 57, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 2)
                {
                    BoxToDraw = new Rectangle(57, 0, 57, BoxToDraw.Height);
                }
                else
                {
                    BoxToDraw = new Rectangle(0, 0, 57, BoxToDraw.Height);
                    _frameCount = 0;
                }

                if (Velocity.X > 0)
                {
                    sp.Draw(StyleSheet, Position, BoxToDraw, enemyColor);
                }
                else
                {
                    sp.Draw(StyleSheet, CollisionRectangle, BoxToDraw, enemyColor, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                }
                
                _frameCount++;

            }
        }


    }
}
