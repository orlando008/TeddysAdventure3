using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class PulseProjectile : GameObject
    {
        private Teddy _teddy;
        private int _frameCount = 0 ;
        private int _framesForCompleteRotation = 50;
        
        private float _rotationAngle;
        private Vector2 _velocity;

        public PulseProjectile(Game game, Vector2 position, Teddy teddy, Vector2 velocity)
            : base(game, position)
        {
            _teddy = teddy;
            _velocity = velocity;
            StyleSheet = game.Content.Load<Texture2D>("Objects\\PulseProjectile");
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
        }

        public override void Draw(GameTime gameTime, SpriteBatch sp)
        {
            if (!Destroyed)
            {

                //todo: make this work based on current velocity, so they roll faster when they are moving faster.
                _rotationAngle = ((float)_frameCount / _framesForCompleteRotation) * 2 * (float)Math.PI;
                var origin = new Vector2(this.BoxToDraw.Height / 2, this.BoxToDraw.Width / 2);

                sp.Draw(this.StyleSheet, this.DestinationBoxToDraw, this.BoxToDraw, Color.White, _rotationAngle, origin, SpriteEffects.None, 0);

                _frameCount++;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Position = new Vector2(Position.X + _velocity.X, Position.Y + _velocity.Y);
        }
    }
}
