using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class OrangeBomb : Enemy
    {
        private int _lengthOfPose = 40;
        private int _lengthOfPoseExplosion = 25;
        private bool _explosionPhase = false;
        public OrangeBomb(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\OrangeBomb");

            Position = position;
            BoxToDraw = new Rectangle(0, 0,25, 25);
            base.Velocity = velocity;
            CanJumpOnToKill = false;
            _gravity = 0;
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            Color enemyColor = Color.White;
            
            int explosionRadius = 0;

            if (this.Dying)
            {
                enemyColor.A = (byte)((1 - ((float)_deathFrameCount / _deathFrames)) * 255);
                BoxToDraw = new Rectangle(0, 0, BoxToDraw.Width, BoxToDraw.Height);
            }
            else
            {

                if (_explosionPhase == false)
                {
                    if (_frameCount < _lengthOfPose)
                    {
                        BoxToDraw = new Rectangle(0, 0, BoxToDraw.Width, BoxToDraw.Height);
                    }
                    else if (_frameCount < _lengthOfPose * 2)
                    {
                        BoxToDraw = new Rectangle(BoxToDraw.Width, 0, BoxToDraw.Width, BoxToDraw.Height);
                    }
                    else if (_frameCount < _lengthOfPose * 3)
                    {
                        BoxToDraw = new Rectangle(BoxToDraw.Width * 2, 0, BoxToDraw.Width, BoxToDraw.Height);
                    }
                    else if (_frameCount < _lengthOfPose * 4)
                    {
                        BoxToDraw = new Rectangle(BoxToDraw.Width * 3, 0, BoxToDraw.Width, BoxToDraw.Height);
                    }
                    else if (_frameCount < _lengthOfPose * 5)
                    {
                        BoxToDraw = new Rectangle(BoxToDraw.Width * 4, 0, BoxToDraw.Width, BoxToDraw.Height);
                    }
                    else if (_frameCount < _lengthOfPose * 10)
                    {
                        if (_frameCount % 4 == 0)
                            BoxToDraw = new Rectangle(BoxToDraw.Width * 5, 0, BoxToDraw.Width, BoxToDraw.Height);
                        else
                            BoxToDraw = new Rectangle(BoxToDraw.Width * 6, 0, BoxToDraw.Width, BoxToDraw.Height);
                    }
                    else
                    {
                        _explosionPhase = true;
                        _frameCount = 0;
                    }
                }

                if (_explosionPhase)
                {
                    if (_frameCount < _lengthOfPoseExplosion * 100)
                    {
                        BoxToDraw = new Rectangle(BoxToDraw.Width * 7, 0, BoxToDraw.Width, BoxToDraw.Height);



                        if (_frameCount < _lengthOfPoseExplosion)
                        {
                            if (_frameCount == 0)
                            {
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X, Position.Y + BoxToDraw.Height, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X, Position.Y - BoxToDraw.Height, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X + BoxToDraw.Width, Position.Y, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X - BoxToDraw.Width, Position.Y, BoxToDraw.Width, BoxToDraw.Height));
                            }
                            explosionRadius = 1;
                        }
                        else if (_frameCount < _lengthOfPoseExplosion * 2)
                        {
                            if (_frameCount == _lengthOfPoseExplosion)
                            {
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X, Position.Y + BoxToDraw.Height * 2, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X, Position.Y - BoxToDraw.Height * 2, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X + BoxToDraw.Width * 2, Position.Y, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X - BoxToDraw.Width * 2, Position.Y, BoxToDraw.Width, BoxToDraw.Height));
                            }
                            explosionRadius = 2;
                        }
                        else
                        {
                            if (_frameCount == _lengthOfPoseExplosion * 2)
                            {
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X, Position.Y + BoxToDraw.Height * 3, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X, Position.Y - BoxToDraw.Height * 3, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X + BoxToDraw.Width * 3, Position.Y, BoxToDraw.Width, BoxToDraw.Height));
                                this.HitBoxes.Add(new GeometryMethods.RectangleF(Position.X - BoxToDraw.Width * 3, Position.Y, BoxToDraw.Width, BoxToDraw.Height));
                            }
                            explosionRadius = 3;
                        }

                    }
                    else
                    {
                        _explosionPhase = false;
                        this.HardKill();
                    }
                }

            }


            if (!Destroyed)
            {

                sp.Draw(StyleSheet, Position, BoxToDraw, enemyColor);

                if (_explosionPhase)
                {
                    if (ChildrenEnemies == null)
                        ChildrenEnemies = new List<Enemy>();

                    foreach (GeometryMethods.RectangleF r in HitBoxes)
                    {
                        sp.Draw(StyleSheet, new Vector2(r.Left, r.Top), BoxToDraw, enemyColor);
                    }
                }

                _frameCount++;

            }

        }
    }
}
