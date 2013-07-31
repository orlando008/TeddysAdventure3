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
        private int _gravitySpeed = 3;
        private int _xVelocity = 1;

        public BowlingBall(Game game, Vector2 position)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\BowlingBall");
          
            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            Destroyed = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch sp)
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

                _frameCount++;
                
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Destroyed)
            {
                return;
            }

            MoveByX(_xVelocity);

            foreach (Rectangle surface in ((Screen)Game.Components[0]).Surfaces)
            {
                if (surface.Intersects(CollisionRectangle))
                {
                    //We must figure out which surface we are hitting to know if we should change direction and which way to shift 
                    if( Math.Abs( surface.Left - CollisionRectangle.Left) > Math.Abs(surface.Left - CollisionRectangle.Right )) {
                        Position = new Vector2(surface.Left - CollisionRectangle.Width - 1,   Position.Y);
                        _xVelocity = -1;
                    }else {
                        Position = new Vector2(surface.Right + 1,   Position.Y);
                        _xVelocity = 1;
                    }

                    break;
                }
            }


            applyGravity();
        }

        private void applyGravity()
        {
            Position = new Vector2(Position.X, Position.Y + _gravitySpeed);

            foreach (Rectangle surfaceRect in ((Screen)Game.Components[0]).Surfaces)
            {
                if (CollisionRectangle.Intersects(surfaceRect) & (CollisionRectangle.Bottom > surfaceRect.Top))
                {
                    Position = new Vector2(Position.X, surfaceRect.Top - BoxToDraw.Height);
                }
            }
        }


    }
}
