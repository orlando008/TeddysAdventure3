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
    public class MatchBoxCar : Enemy
    {
        private int _frameCount;
        private int _lengthOfPose = 12;
        private int _gravitySpeed = 3;
        private Rectangle _mySurface;

        public MatchBoxCar(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\MatchBoxCar");

            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            Destroyed = false;
            base.Velocity = velocity;
            CanJumpOnToKill = true;
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

            MoveByX((int)Velocity.X);

            //wait till this car lands on a surface, then you only need to check against that surface (for whether or not you hit the end of the surface)
            if (_mySurface != null)
            {
                if (_mySurface.Left > CollisionRectangle.Left | _mySurface.Right < CollisionRectangle.Right)
                {
                    Velocity = new Vector2(-Velocity.X, Velocity.Y);
                }
            }

            //check for bumping into other surfaces that are laying on top of the main surface
            foreach (Rectangle surface in ((Screen)Game.Components[0]).Surfaces)
            {
                if (surface.Intersects(CollisionRectangle))
                {
                    //We must figure out which surface we are hitting to know if we should change direction and which way to shift 
                    if (Math.Abs(surface.Left - CollisionRectangle.Left) > Math.Abs(surface.Left - CollisionRectangle.Right))
                    {
                        Position = new Vector2(surface.Left - CollisionRectangle.Width - 1, Position.Y);
                        Velocity = new Vector2(-Velocity.X, Velocity.Y);
                    }
                    else
                    {
                        Position = new Vector2(surface.Right + 1, Position.Y);
                        Velocity = new Vector2(-Velocity.X, Velocity.Y);
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
                    //once it hits a base surface for the first time, claim that surface as "_mySurface", stop applying gravity
                    _mySurface = surfaceRect;
                    Position = new Vector2(Position.X, surfaceRect.Top - BoxToDraw.Height);
                }
            }
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            if (!Destroyed)
            {
                if (_frameCount < _lengthOfPose * 1)
                {
                    BoxToDraw = new Rectangle(0, 0, 67, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 2)
                {
                    BoxToDraw = new Rectangle(67, 0, 67, BoxToDraw.Height);
                }
                else
                {
                    BoxToDraw = new Rectangle(0, 0, 67, BoxToDraw.Height);
                    _frameCount = 0;
                }

                if (Velocity.X > 0)
                {
                    sp.Draw(StyleSheet, CollisionRectangle, BoxToDraw, Color.White,0,new Vector2(0,0), SpriteEffects.FlipHorizontally, 0);
                }
                else
                {
                    sp.Draw(StyleSheet, Position, BoxToDraw, Color.White);
                }
                

                _frameCount++;

            }
        }


    }
}
