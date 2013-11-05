using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class BowlingBall:Enemy
    {
        private int _frameCount;
        private int _framesForCompleteRotation = 48;
        private float _rotationAngle;
        private int _framesUntilIdleDeath = 10;
        private int _idleDeathCounter = 0;
       
        public BowlingBall(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\BowlingBall");
          
            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            base.Velocity = velocity;
            this._collisionDampingFactor = .3f;
            CanJumpOnToKill = true;
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            base.DrawEnemy(gameTime, sp);

            if (!Destroyed)
            {

                //todo: make this work based on current velocity, so they roll faster when they are moving faster.
                _rotationAngle = ((float)_frameCount / _framesForCompleteRotation) * 2 * (float)Math.PI;
                this.BoxToDraw = new Rectangle(0, 0, 50, BoxToDraw.Height);
                var origin = new Vector2(this.BoxToDraw.Height/2, this.BoxToDraw.Width/2);

                sp.Draw(this.StyleSheet, this.DestinationBoxToDraw, this.BoxToDraw, Color.White, _rotationAngle, origin, SpriteEffects.None, _layerDepth);

                if (this._velocity.X > 0)
                    _frameCount++;
                else if (this._velocity.X < 0)
                    _frameCount--;

#if COLLISIONS
                sp.Draw(this._redFill, this.CollisionRectangle, null, Color.Red);
#endif
            }


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Velocity.X == 0)
            {
                _idleDeathCounter++;
               
            }

            if (_idleDeathCounter >= _framesUntilIdleDeath)
            {
                this.HardKill();
                _idleDeathCounter = 0;
            }
        }

    }
}
