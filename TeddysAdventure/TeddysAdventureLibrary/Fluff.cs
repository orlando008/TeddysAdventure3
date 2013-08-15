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
        private int _centerLine;
        private int _lengthOfPose = 20;


        public Fluff(Game game, Vector2 position)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fluff");
          
            _centerLine = (int)position.X;
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
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
                
            }
        }

        public override void MoveGameObjectByX(int x)
        {
            this.Position = new Vector2((int)Position.X + x, (int)Position.Y);
            _centerLine = _centerLine + x;
        }
    }
}
