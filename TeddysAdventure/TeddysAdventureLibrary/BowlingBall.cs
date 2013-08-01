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
        private int _lengthOfPose = 12;


        public BowlingBall(Game game, Vector2 position)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\BowlingBall");
          
            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            Destroyed = false;
            this._collisionDampingFactor = .3f;
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            if (!Destroyed)
            {
                if (_frameCount < _lengthOfPose * 1)
                {
                    BoxToDraw = new Rectangle(0, 0, 50, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 2)
                {
                    BoxToDraw = new Rectangle(50, 0, 50, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 2)
                {
                    BoxToDraw = new Rectangle(100, 0, 50, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 2)
                {
                    BoxToDraw = new Rectangle(150, 0, 50, BoxToDraw.Height);
                }
                else
                {
                    BoxToDraw = new Rectangle(0, 0, 50, BoxToDraw.Height);
                    _frameCount = 0;
                }

                sp.Draw(StyleSheet, Position, BoxToDraw, Color.White);

                if ( this._velocity.X != 0 ) {
                 _frameCount++;
                }
            }
        }




    }
}
