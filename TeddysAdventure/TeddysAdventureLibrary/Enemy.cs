using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class Enemy:DrawableGameComponent
    {
        private Texture2D _styleSheet;
        private Vector2 _position;
        private Rectangle _boxToDraw;
        private Vector2 _frameSize;
        private bool _destroyed;
        private bool _canJumpOnToKill;

        private float _gravity = .3f; //m/s2
        protected Vector2 _velocity = new Vector2(1, -3);
        protected float _collisionDampingFactor = .6f;

        protected float _layerDepth = 0;   //0 = front of screen, 1 = back  

        public Texture2D StyleSheet
        {
            get { return _styleSheet; }
            set { _styleSheet = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public Vector2 FrameSize
        {
            get { return _frameSize; }
            set { _frameSize = value; }
        }

        public Rectangle BoxToDraw
        {
            get { return _boxToDraw; }
            set { _boxToDraw = value; }
        }

        public Rectangle DestinationBoxToDraw
        {
            get
            {
                return new Rectangle((int)_position.X + _boxToDraw.Width / 2, (int)_position.Y + _boxToDraw.Height / 2, (int)_boxToDraw.Width, (int)_boxToDraw.Height);
            }
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, BoxToDraw.Width, BoxToDraw.Height);
            }
        }

        public bool Destroyed
        {
            get { return _destroyed; }
            set
            {
                _destroyed = value;
                if (value == true)
                {
                    Position = new Vector2(-100, -100);
                }
            }
        }

        public bool CanJumpOnToKill
        {
            get { return _canJumpOnToKill; }
            set
            {
                _canJumpOnToKill = value;
            }
        }

        public Enemy(Game game)
            : base(game)
        {

        }

        public void MoveByX(int x)
        {
            this.Position = new Vector2((int)Position.X + x, (int)Position.Y);
        }

        public void MoveByY(int y)
        {
            this.Position = new Vector2((int)Position.X, (int)Position.Y + y);
        }

        public virtual void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            base.Draw(gameTime);
        }


        public override void Update(GameTime gameTime)
        {
            if (Destroyed)
            {
                return;
            }

            MoveByX((int) _velocity.X);

            
            foreach (Rectangle surface in ((Screen)Game.Components[0]).Surfaces)
            {

                if (surface.Intersects(CollisionRectangle))
                {
                    //We must figure out which surface we are hitting to know if we should change direction and which way to shift 
                    if (Math.Abs(surface.Left - CollisionRectangle.Left) > Math.Abs(surface.Left - CollisionRectangle.Right))
                    {
                        Position = new Vector2(surface.Left - CollisionRectangle.Width - 1, Position.Y);
                        if (_velocity.X > 0 ) 
                             _velocity.X *= - _collisionDampingFactor;
                    }
                    else
                    {
                        Position = new Vector2(surface.Right + 1, Position.Y);
                        if (_velocity.X < 0)
                            _velocity.X *=  - _collisionDampingFactor;
                    }

                    if (Math.Abs(_velocity.X) < 1)
                        _velocity.X = 0f;
                    break;
                }
            }


            applyGravity();
        }

        private void applyGravity()
        {

            MoveByY((int)_velocity.Y);
            _velocity.Y += _gravity; //Down is positive

            foreach (Rectangle surfaceRect in ((Screen)Game.Components[0]).Surfaces)
            {
                if (CollisionRectangle.Intersects(surfaceRect) & (CollisionRectangle.Bottom > surfaceRect.Top))
                {
                    Position = new Vector2(Position.X, surfaceRect.Top - BoxToDraw.Height);
                    _velocity.Y *= -_collisionDampingFactor;

                    if (Math.Abs(_velocity.Y) < 1)
                        _velocity.Y = 0f;

                    break;
                }
            }
        }

    }
}
