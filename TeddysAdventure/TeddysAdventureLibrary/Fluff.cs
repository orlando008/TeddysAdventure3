using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class Fluff : GameObject
    {
        private int _floatCount;
        private float _centerLine;
        private int _lengthOfPose = 20;
        private bool _applyGravity = false;

        private float _yVelocity;
        private float _xVelocity;

        private float _gravity = .10f;

        public Fluff(Game game, Vector2 position)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");
          
            _centerLine = position.X;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
        }

        public Fluff(Game game, Vector2 position, bool applyGravity)
            : base(game, position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");

            _centerLine = (int)position.X;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);

            _applyGravity = applyGravity;

            // Give initial velocities
            Random r = new Random();
            _yVelocity = (float)r.NextDouble() * -10;
            _xVelocity = (float)r.NextDouble() * r.Next(-1, 2) * -5;
        }

        private GeometryMethods.RectangleF fluffRect
        {
            get
            {
                return new GeometryMethods.RectangleF(Position.X, Position.Y, BoxToDraw.Width, BoxToDraw.Height);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Destroyed)
            {
                if (_floatCount < _lengthOfPose)
                {
                    Position = new Vector2(_centerLine, Position.Y);
                    _floatCount++;
                }
                else if (_floatCount < _lengthOfPose * 2)
                {
                    Position = new Vector2(_centerLine - 1, Position.Y);
                    _floatCount++;
                }
                else if (_floatCount < _lengthOfPose * 3)
                {
                    Position = new Vector2(_centerLine, Position.Y);
                    _floatCount++;
                }
                else if (_floatCount < _lengthOfPose * 4)
                {
                    Position = new Vector2(_centerLine + 1, Position.Y);
                    _floatCount++;
                }
                else
                {
                    _floatCount = 0;
                }

                if (_applyGravity)
                {
                    // Change gravity and lower the x velocity
                    _yVelocity += _gravity;
                    _centerLine += _xVelocity;

                    Position = new Vector2(Position.X + _xVelocity, Position.Y + _yVelocity);
                    foreach (Rectangle surfaceRect in ((Screen)Game.Components[0]).Surfaces)
                    {
                        // if we are rising, check for top surfaces
                        if (_yVelocity < 0)
                        {
                            if (fluffRect.Intersects(surfaceRect) & (fluffRect.Top < surfaceRect.Bottom))
                            {
                                Position = new Vector2(Position.X + _xVelocity, surfaceRect.Bottom + 1);

                                _yVelocity = 0.0f;
                            }
                        }
                        else
                        {
                            if (fluffRect.Intersects(surfaceRect) & (fluffRect.Bottom > surfaceRect.Top))
                            {
                                Position = new Vector2(Position.X + _xVelocity, surfaceRect.Top - BoxToDraw.Height);
                                _yVelocity = 0.0f;
                                _xVelocity = 0.0f;
                                _applyGravity = false;
                            }
                        }
                    }
                }

                if (Position.Y > Game.GraphicsDevice.Viewport.Height)
                {
                    _applyGravity = false;
                    Destroyed = true;
                }
            }
        }

        public override void MoveGameObjectByX(int x)
        {
            this.Position = new Vector2((int)Position.X + x, (int)Position.Y);
            _centerLine = _centerLine + x;
        }
    }
}
