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
            bool explosionPhase = false;
            int explosionRadius = 0;

            if (this.Dying)
            {
                enemyColor.A = (byte)((1 - ((float)_deathFrameCount / _deathFrames)) * 255);
                BoxToDraw = new Rectangle(0, 0, BoxToDraw.Width, BoxToDraw.Height);
            }
            else
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
                else if (_frameCount < _lengthOfPose * 13)
                {
                    BoxToDraw = new Rectangle(BoxToDraw.Width * 7, 0, BoxToDraw.Width, BoxToDraw.Height);
                    explosionPhase = true;


                    if (_frameCount < _lengthOfPose * 11)
                    {
                        explosionRadius = 1;
                    }
                    else if (_frameCount < _lengthOfPose * 12)
                    {
                        explosionRadius = 2;
                    }
                    else
                    {
                        explosionRadius = 3;
                    }
                   
                }
                else
                {
                    explosionPhase = false;
                    this.HardKill();
                }
            }


            if (!Destroyed)
            {

                sp.Draw(StyleSheet, Position, BoxToDraw, enemyColor);

                if (explosionPhase)
                {
                    if (explosionRadius > 0)
                    {
                        sp.Draw(StyleSheet, new Vector2(Position.X, Position.Y + BoxToDraw.Height), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X, Position.Y - BoxToDraw.Height), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X + BoxToDraw.Width, Position.Y), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X - BoxToDraw.Width, Position.Y), BoxToDraw, enemyColor);
                    }

                    if (explosionRadius > 1)
                    {
                        sp.Draw(StyleSheet, new Vector2(Position.X, Position.Y + BoxToDraw.Height * 2), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X, Position.Y - BoxToDraw.Height * 2), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X + BoxToDraw.Width * 2, Position.Y), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X - BoxToDraw.Width * 2, Position.Y), BoxToDraw, enemyColor);
                    }

                    if (explosionRadius > 2)
                    {
                        sp.Draw(StyleSheet, new Vector2(Position.X, Position.Y + BoxToDraw.Height * 3), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X, Position.Y - BoxToDraw.Height * 3), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X + BoxToDraw.Width * 3, Position.Y), BoxToDraw, enemyColor);
                        sp.Draw(StyleSheet, new Vector2(Position.X - BoxToDraw.Width * 3, Position.Y), BoxToDraw, enemyColor);
                    }
                }

                _frameCount++;

            }

        }
    }
}
