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
        private bool _floats = true;
        private float _centerLine;
        private int _lengthOfPose = 20;
        private bool _applyGravity = false;

        private float _yVelocity;
        private float _xVelocity;

        private float _xAccelleration;
        private float _yAccelleration;

        private float _gravity = .10f;

        private bool _isTeddyFluff = false;


        public Fluff(Game game, Vector2 position)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");
          
            _centerLine = position.X;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
        }

        public Fluff(Game game, Vector2 position, bool applyGravity, float xVelocty, float yVelocity, bool floats)
            : this(game, position, applyGravity, xVelocty, yVelocity, null, false, floats )
        {

        }

        public Fluff(Game game, Vector2 position, bool applyGravity, float xVelocty, float yVelocity, Rectangle? drawBox, bool isTeddyFluff, bool floats)
            : base(game, position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");

            _centerLine = (int)position.X;
            BoxToDraw = (drawBox == null) ? new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height):drawBox.Value  ;

            _applyGravity = applyGravity;
            _isTeddyFluff = isTeddyFluff;
            _xVelocity = xVelocty;
            _yVelocity = yVelocity;
            _floats = floats;
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

        public void SetApplyGravity(bool apply)
        {
            _applyGravity = apply;
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
                if (_floats)
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
                    foreach (Surface surface in currentScreen.Surfaces )
                    {

                        // if we are rising, check for top surfaces
                        if (_yVelocity <0)
                        {
                            if (fluffRect.Intersects(surface.Rect) & (fluffRect.Top < surface.Bottom  && fluffRect.Bottom > surface.Bottom))
                            {
                                Position = new Vector2(Position.X + _xVelocity, surface.Bottom + 1);
                                _yVelocity = 0.0f;
                            }
                        }
                        else
                        {
                            if (fluffRect.Intersects(surface.Rect) & (fluffRect.Bottom > surface.Top))
                            {
                                Position = new Vector2(Position.X + _xVelocity, surface.Top - BoxToDraw.Height);

                                if (_applyGravity)
                                {
                                    _yVelocity = 0.0f;

                                    if (!_isTeddyFluff)
                                        _xVelocity = 0;
                                } 

                            }
                        }

                        if (_xVelocity < 0)
                        {
                            //Moving Left
                            if (fluffRect.Intersects(surface.Rect) && fluffRect.Left < surface.Right && fluffRect.Right > surface.Right)
                            {
                                this.Position = new Vector2(surface.Right + 1, this.Position.Y);
                                _xVelocity = 0;
                            }
                        }
                        else if (_xVelocity > 0)
                        {
                            if (fluffRect.Intersects(surface.Rect) && fluffRect.Right > surface.Left && fluffRect.Left < surface.Left)
                            {
                                this.Position = new Vector2(surface.Left - fluffRect.Width, this.Position.Y);
                                _xVelocity = 0;
                            }
                        }

                    }
                }

                if (Position.Y > currentScreen.LevelHeight)
                {
                    _applyGravity = false;

                    if (!_isTeddyFluff) { Destroyed = true; }
                }
            }
        }

    }
}
