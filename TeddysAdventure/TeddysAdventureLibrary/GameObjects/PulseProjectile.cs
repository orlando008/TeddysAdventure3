using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class PulseProjectile : GameObject
    {
        private Teddy _teddy;
        private int _frameCount = 0 ;
        private int _framesForCompleteRotation = 6;
        
        private float _rotationAngle;
        private Vector2 _velocity;
        private float _powerLevelSpeed;
        private int _powerLevelSizeBoost;

        public PulseProjectile(Game game, Vector2 position, Teddy teddy, Vector2 velocity, float powerLevelSpeed, int powerLevelSize)
            : base(game, position)
        {
            _teddy = teddy;
            _velocity = velocity;
            _powerLevelSpeed = powerLevelSpeed;
            _powerLevelSizeBoost = powerLevelSize;
            StyleSheet = game.Content.Load<Texture2D>("Objects\\PulseProjectile");
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width / 2 + _powerLevelSizeBoost, StyleSheet.Height + _powerLevelSizeBoost);
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch sp)
        {
            if (!Destroyed)
            {
                if (_frameCount <= _framesForCompleteRotation)
                {
                    sp.Draw(this.StyleSheet, new Rectangle((int)this.Position.X, (int)this.Position.Y, BoxToDraw.Width, BoxToDraw.Height), new Rectangle(0, 0, StyleSheet.Width / 2, StyleSheet.Height), Color.White);
                }
                else if (_frameCount <= _framesForCompleteRotation * 2)
                {
                    sp.Draw(this.StyleSheet, new Rectangle((int)this.Position.X, (int)this.Position.Y, BoxToDraw.Width, BoxToDraw.Height), new Rectangle(StyleSheet.Width / 2, 0, StyleSheet.Width / 2, StyleSheet.Height), Color.White);
                }
                else
                {
                    sp.Draw(this.StyleSheet, new Rectangle((int)this.Position.X, (int)this.Position.Y, BoxToDraw.Width, BoxToDraw.Height), new Rectangle(0, 0, StyleSheet.Width / 2, StyleSheet.Height), Color.White);
                    _frameCount = -1;
                }

                _frameCount++;

#if COLLISIONS
                sp.Draw(this._redFill, this.CollisionRectangleRegular, null, Color.Red);
#endif
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Destroyed)
            {
                Screen currentScreen = (Screen)Game.Components[0];
                base.Update(gameTime);

                if (_velocity.X < 0)
                {
                    Position = new Vector2(Position.X + _velocity.X - _powerLevelSpeed, Position.Y + _velocity.Y);
                }
                else
                {
                    Position = new Vector2(Position.X + _velocity.X + _powerLevelSpeed, Position.Y + _velocity.Y);
                }
                

                if (Position.X > currentScreen.LevelWidth || Position.X < 0 || Position.Y > currentScreen.LevelHeight || Position.Y < 0)
                    this.Destroyed = true;
                else
                    checkForEnemyCollisions(currentScreen);
                
            }
        }

        private void checkForEnemyCollisions(Screen currentScreen)
        {
            foreach (Enemy e in currentScreen.Enemies)
            {
                checkForEnemyCollisionRecursive(currentScreen,e);
            }
        }

        private void checkForEnemyCollisionRecursive(Screen currentScreen, Enemy e)
        {
            if (!e.Destroyed & e.CollisionRectangle.Intersects(this.CollisionRectangleRegular))
            {
                e.Kill();
                this.Destroyed = true;
            }

            if (e.ChildrenEnemies != null)
            {
                foreach (Enemy e2 in e.ChildrenEnemies)
                {
                    checkForEnemyCollisionRecursive(currentScreen, e2);
                }
            }

        }

    }
}
