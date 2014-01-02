using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    /// <summary>
    /// This enemy floats in the air near the top of the screen and at random intervals drops "A-Bombs" down.
    /// </summary>
    public class FlyingBook : Enemy
    {
        private ABomb _aBomb;
        private SpriteFont _hudFont;
        private int _lengthOfPose = 6;
        private int _bombTicks = 0;

        public FlyingBook(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\FlyingBook");
            _hudFont = game.Content.Load<SpriteFont>("Fonts\\Arial12");
            Position = position;
            BoxToDraw = new Rectangle(0, 0, 103, StyleSheet.Height);
            base.Velocity = velocity;
            CanJumpOnToKill = true;
            this._playerCanPassThrough = true;

            //Create the book's A-Bomb, put it off screen with no velocity
            _aBomb = new ABomb(game, new Vector2(-100, -100), new Vector2(0, 0));
            this.ChildrenEnemies = new List<Enemy>();
            this.ChildrenEnemies.Add(_aBomb);
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            //base.DrawEnemy(gameTime, sp);

            //For now i will just show text counting the frames

            Color enemyColor = Color.White;

            if (this.Dying)
                enemyColor.A = (byte)((1 - ((float)_deathFrameCount / _deathFrames)) * 255);


            if (!Destroyed)
            {
                if (_frameCount < _lengthOfPose * 1)
                {
                    BoxToDraw = new Rectangle(0, 0, 150, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 2)
                {
                    BoxToDraw = new Rectangle(150, 0, 150, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 3)
                {
                    BoxToDraw = new Rectangle(300, 0, 150, BoxToDraw.Height);
                }
                else if (_frameCount < _lengthOfPose * 4)
                {
                    BoxToDraw = new Rectangle(150, 0, 150, BoxToDraw.Height);
                    _frameCount = 0;
                }

                sp.Draw(StyleSheet, Position, BoxToDraw, enemyColor);

                _frameCount++;

            }

            _aBomb.DrawEnemy(gameTime, sp);
        }

        public override void Update(GameTime gameTime)
        {
            Random r = new Random();
            if (_bombTicks > r.Next(25, 150))
            {
                if (_aBomb.Position.Y >= this.Game.GraphicsDevice.Viewport.Height | _aBomb.Position.Y < 0)
                {
                    _aBomb.Position = new Vector2(Position.X, Position.Y);
                    _aBomb.Velocity = new Vector2(0, 3);
                    _aBomb.BringToLife();
                    _bombTicks = 0;
                }
            }

            Position = new Vector2(Position.X + Velocity.X, Position.Y);

            if (Position.X <= 0 || Position.X >= this.Game.GraphicsDevice.Viewport.Width - this.BoxToDraw.Width)
            {
                if (Position.X <= 0)
                {
                    Position = new Vector2(0, Position.Y);
                }
                else
                {
                    Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width - this.BoxToDraw.Width, Position.Y);
                }
                
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            }


            

            _bombTicks++;

            if (_aBomb.Position.Y < this.Game.GraphicsDevice.Viewport.Height && _aBomb.Position.Y > 0)
            {
                _aBomb.Update(gameTime);
            }
            
        }
    }
}
