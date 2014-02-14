﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class NightmareToaster : Enemy, ISurfaceInterface
    {

        private int _lengthOfPose = 20;
        private int _damage = 10;

        public NightmareToaster(Game game, Vector2 position, Vector2 velocity)
            : base(game) 
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\Toaster");

            Position = position;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            base.Velocity = velocity;
            this._canJumpOnToKill = false;
            this._gravity = 0f;
            this._collisionDampingFactor = .3f;
            this._playerCanRide = false;
            base.Damage = _damage;
            this._passesThroughSurfaces = true;

        }

        

        Rectangle ISurfaceInterface.SurfaceBounds()
        {
            return this.CollisionRectangle;
        }

        Vector2 ISurfaceInterface.SurfaceVelocity()
        {
            return this.Velocity;
        }

        Enemy ISurfaceInterface.SurfaceOwner()
        {
            return this;
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            base.DrawEnemy(gameTime, sp);

            if (!Destroyed)
            {


                if (_frameCount < _lengthOfPose * 1)
                {
                    BoxToDraw = new Rectangle(0, 0, 80, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 2)
                {
                    BoxToDraw = new Rectangle(80, 0, 80, BoxToDraw.Height);
                }
                else
                {
                    BoxToDraw = new Rectangle(0, 0, 80, BoxToDraw.Height);
                    _frameCount = 0;
                }

#if COLLISIONS
                sp.Draw(_redFill, this.CollisionRectangle, null, Color.Red);
#endif

                var origin = new Vector2(0,0);

                if (this._velocity.X > 0)
                {
                    sp.Draw(this.StyleSheet, this.CollisionRectangle, this.BoxToDraw, Color.White, 0, origin, SpriteEffects.FlipHorizontally, _layerDepth);
                }
                else if (this._velocity.X < 0)
                {
                    sp.Draw(this.StyleSheet, this.CollisionRectangle, this.BoxToDraw, Color.White, 0, origin, SpriteEffects.None, _layerDepth);

                }



                _frameCount++;
            }


        }




    }
}