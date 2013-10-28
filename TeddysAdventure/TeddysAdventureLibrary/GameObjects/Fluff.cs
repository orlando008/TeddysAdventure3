﻿using System;
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

        private float _xAccelleration;
        private float _yAccelleration;

        private float _gravity = .10f;

        public Fluff(Game game, Vector2 position)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");
          
            _centerLine = position.X;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
        }

        public Fluff(Game game, Vector2 position, bool applyGravity, float xVelocty, float yVelocity)
            : base(game, position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");

            _centerLine = (int)position.X;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);

            _applyGravity = applyGravity;

            _xVelocity = xVelocty;
            _yVelocity = yVelocity;
        }

        public Fluff(Game game, Vector2 position, bool applyGravity, float xVelocty, float yVelocity, Rectangle drawBox)
            : base(game, position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");

            _centerLine = (int)position.X;
            BoxToDraw = drawBox;

            _applyGravity = applyGravity;

            _xVelocity = xVelocty;
            _yVelocity = yVelocity;
        }


        private GeometryMethods.RectangleF fluffRect
        {
            get
            {
                return new GeometryMethods.RectangleF(Position.X, Position.Y, BoxToDraw.Width, BoxToDraw.Height);
            }
        }

        public void SetAccelleration(Vector2 accel)
        {
            _xAccelleration = accel.X;
            _yAccelleration = accel.Y;
        }

        public void SetVelocity(Vector2 velocity)
        {
            _xVelocity = velocity.X;
            _yVelocity = velocity.Y;
        }

        public void SetPosition(Vector2 pos)
        {
            this.Position = pos;
            _centerLine = pos.X;
        }

        public Vector2 Velocity { get { return new Vector2(_xVelocity, _yVelocity); } }
        public Vector2 Acceleration { get { return new Vector2(_xAccelleration, _yAccelleration); } }

        public override void Update(GameTime gameTime)
        {

            Screen currentScreen = (Screen)Game.Components[0];

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


                if (_xAccelleration != 0)
                    _xVelocity += _xAccelleration;

                if (_yAccelleration != 0)
                    _yVelocity += _yAccelleration;


                if (_applyGravity || _xAccelleration != 0  || _yAccelleration != 0)
                {
                    // Change gravity and lower the x velocity
                    if ( _applyGravity)
                        _yVelocity += _gravity;
                    
                    _centerLine += _xVelocity;

                    Position = new Vector2(Position.X + _xVelocity, Position.Y + _yVelocity);
                    foreach (Surface surfaceRect in currentScreen.Surfaces )
                    {

                        //todo: handle left/right surface detection.

                        // if we are rising, check for top surfaces
                        if (_yVelocity < 0)
                        {
                            if (fluffRect.Intersects(surfaceRect.Rect) & (fluffRect.Top < surfaceRect.Bottom))
                            {
                                Position = new Vector2(Position.X + _xVelocity, surfaceRect.Bottom + 1);
                                _yVelocity = 0.0f;
                            }
                        }
                        else
                        {
                            if (fluffRect.Intersects(surfaceRect.Rect) & (fluffRect.Bottom > surfaceRect.Top))
                            {
                                Position = new Vector2(Position.X + _xVelocity, surfaceRect.Top - BoxToDraw.Height);

                                if (_applyGravity)
                                {
                                    _yVelocity = 0.0f;
                                    _xVelocity = 0.0f;
                                    _applyGravity = false;
                                } 

                            }
                        }
                    }
                }

                if (Position.Y > ((Screen)Game.Components[0]).LevelHeight)
                {
                    _applyGravity = false;
                    Destroyed = true;
                }
            }
        }

    }
}
